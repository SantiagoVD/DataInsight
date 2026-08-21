import { useEffect, useRef, useState } from 'react';
import { Activity, ArrowRight, BarChart3, Check, Clipboard, CloudUpload, Download, FileSpreadsheet, LoaderCircle, RotateCcw, ShieldCheck, Sparkles, Trophy, X } from 'lucide-react';
import { createJob, createUploadUrl, uploadFileToS3 } from './services/dataInsightApi';
import { useJobPolling } from './hooks/useJobPolling';
import type { ProcessingResult, ProductMetric } from './types/dataInsight';

type View = 'upload' | 'processing' | 'complete' | 'error';
const steps = ['Archivo seleccionado', 'URL generada', 'Archivo subido', 'Trabajo creado', 'Analizando datos'];
const numberFormat = new Intl.NumberFormat('en-US');

function formatSize(size: number) {
  if (size < 1024) return `${size} B`;
  return `${(size / 1024).toFixed(1)} KB`;
}

function friendlyError(error: unknown) {
  const message = error instanceof Error ? error.message : 'Something went wrong. Please try again.';
  if (message.toLowerCase().includes('failed to fetch')) return 'No se pudo conectar con DataInsight. Verifica la conexión con la API e inténtalo nuevamente.';
  if (message.includes('403')) return 'El permiso de carga expiró. Selecciona el archivo nuevamente e inténtalo otra vez.';
  if (message.toLowerCase().includes('s3 upload failed')) return 'No se pudo subir el archivo a S3. Verifica el permiso de carga e inténtalo nuevamente.';
  if (message.toLowerCase().includes('internal server error')) return 'El servicio de procesamiento encontró un error. Inténtalo nuevamente más tarde.';
  if (message.toLowerCase().startsWith('request failed')) return 'La solicitud al servicio no pudo completarse. Inténtalo nuevamente.';
  return message;
}

function App() {
  const input = useRef<HTMLInputElement>(null);
  const [file, setFile] = useState<File | null>(null);
  const [view, setView] = useState<View>('upload');
  const [step, setStep] = useState(0);
  const [error, setError] = useState('');
  const [jobId, setJobId] = useState<string | null>(null);
  const [drag, setDrag] = useState(false);
  const [copied, setCopied] = useState(false);
  const { result, error: pollingError, timedOut, retry } = useJobPolling(jobId, view === 'processing');

  useEffect(() => {
    if (result && view === 'processing') setView('complete');
  }, [result, view]);

  const selectFile = (candidate?: File) => {
    if (!candidate) return;
    if (!candidate.name.toLowerCase().endsWith('.csv')) {
      setError('Selecciona únicamente un archivo CSV. Los archivos de Excel (.xlsx) todavía no son compatibles.');
      return;
    }
    setFile(candidate); setError(''); setView('upload'); setStep(1); setJobId(null);
  };

  const analyze = async () => {
    if (!file) return;
    const contentType = file.type || 'text/csv';
    setView('processing'); setError('');
    try {
      const upload = await createUploadUrl(file.name, contentType); setStep(2);
      await uploadFileToS3(upload.UploadUrl, file, contentType); setStep(3);
      const job = await createJob(file.name, contentType, upload.ObjectKey); setStep(5); setJobId(job.JobId);
    } catch (uploadError) {
      setView('error'); setError(friendlyError(uploadError));
    }
  };

  const reset = () => { setFile(null); setJobId(null); setView('upload'); setStep(0); setError(''); setCopied(false); };
  const copyJobId = async () => { if (!jobId) return; await navigator.clipboard?.writeText(jobId); setCopied(true); window.setTimeout(() => setCopied(false), 1600); };

  return <main>
    <nav><div className="brand"><span className="brand-mark"><BarChart3 size={19} /></span><span>Data<span>Insight</span></span></div><div className="nav-status"><span className="pulse" /> Analítica serverless <span className="divider" /><ShieldCheck size={15} /> Protegido por AWS</div></nav>
    {view === 'complete' && result ? <Dashboard result={result} onReset={reset} jobId={jobId} copied={copied} onCopy={copyJobId} /> : <section className="hero">
      <div className="eyebrow"><Sparkles size={15} /> ANALÍTICA DE DATOS, REIMAGINADA</div><h1>Convierte tus datos en<br /><em>decisiones claras.</em></h1><p className="subtitle">Transforma tus datos CSV en resultados accionables con un flujo serverless rápido y seguro.</p>
      {view === 'processing' ? <ProcessingState jobId={jobId} step={step} /> : <>
        <div className={`dropzone ${drag ? 'drag' : ''}`} onDragOver={e => { e.preventDefault(); setDrag(true); }} onDragLeave={() => setDrag(false)} onDrop={e => { e.preventDefault(); setDrag(false); selectFile(e.dataTransfer.files[0]); }} onClick={() => input.current?.click()} role="button" tabIndex={0} onKeyDown={e => { if (e.key === 'Enter' || e.key === ' ') input.current?.click(); }}>
          <input ref={input} type="file" accept=".csv,text/csv" hidden onChange={e => selectFile(e.target.files?.[0])} aria-label="Seleccionar un archivo CSV" />
          <div className="upload-icon"><CloudUpload size={28} /></div><h3>{file ? 'CSV listo para analizar' : 'Arrastra tu archivo CSV aquí'}</h3><p>{file ? 'Haz clic para elegir otro archivo' : 'o búscalo en tu computadora'}</p><button type="button" className="browse-button" onClick={e => { e.stopPropagation(); input.current?.click(); }}>Seleccionar CSV</button><small>Solo archivos CSV · Máximo 50 MB</small>
        </div>
        {file && <div className="file-card"><div className="file-symbol"><FileSpreadsheet size={23} /></div><div className="file-info"><strong>{file.name}</strong><span>{formatSize(file.size)} <b>·</b> CSV</span></div><button className="icon-btn" aria-label="Remove selected file" onClick={e => { e.stopPropagation(); reset(); }}><X size={18} /></button></div>}
        {(error || pollingError || timedOut) && <div className="error" role="alert">{timedOut ? 'El procesamiento está tardando más de lo esperado. Puedes volver a consultar el trabajo o intentarlo más tarde.' : error || friendlyError(pollingError)}{pollingError && <button type="button" onClick={retry}>Reintentar</button>}</div>}
        {view === 'error' && <button className="secondary retry-button" onClick={analyze}><RotateCcw size={16} /> Reintentar carga</button>}
        {view !== 'error' && <button className="primary" disabled={!file} onClick={analyze}>Analizar datos <ArrowRight size={18} /></button>}
      </>}
      <Progress steps={steps} current={step} />
      <ExamplePanel />
      <div className="features"><span><ShieldCheck size={17} /> Cargas seguras</span><span><Activity size={17} /> Procesamiento serverless</span><span><BarChart3 size={17} /> Insights accionables</span></div>
    </section>}
    <footer>© 2025 DataInsight <span>Decisiones basadas en datos.</span></footer>
  </main>;
}

function Progress({ steps: labels, current }: { steps: string[]; current: number }) { return <div className="progress">{labels.map((label, index) => <div className={`progress-step ${index < current ? 'complete' : ''} ${index === current ? 'active' : ''}`} key={label}><span>{index < current ? <Check size={13} /> : index + 1}</span>{label}</div>)}</div>; }

function ProcessingState({ jobId, step }: { jobId: string | null; step: number }) { return <div className="processing-card"><div className="processing-icon"><LoaderCircle className="spin" size={30} /></div><span className="success-label">PROCESAMIENTO INICIADO</span><h2>Analizando tus datos</h2><p>Tu archivo CSV se subió correctamente y está siendo analizado.</p><div className="job-line"><span>ID DEL TRABAJO</span><strong>{jobId || 'Creando...'}</strong></div><div className="status-line"><i /> PROCESANDO</div><p className="async-note"><Activity size={16} /> Los resultados se generan de forma asíncrona en DataInsight.</p><Progress steps={steps} current={step} /></div>; }

function ExamplePanel() { return <section className="example-panel"><div className="example-heading"><div><span className="eyebrow left"><FileSpreadsheet size={14} /> ESTRUCTURA CSV</span><h2>¿Necesitas un ejemplo?</h2><p>Descarga un archivo de muestra para conocer la estructura esperada.</p></div><a className="download-button" href="/examples/data-insight-example.csv" download><Download size={16} /> Descargar CSV de ejemplo</a></div><div className="table-wrap"><table><thead><tr><th>producto</th><th>cantidad</th><th>precio</th></tr></thead><tbody><tr><td>Laptop</td><td>2</td><td>3500</td></tr><tr><td>Mouse</td><td>10</td><td>80</td></tr><tr><td>Teclado</td><td>5</td><td>150</td></tr></tbody></table></div><div className="required-columns"><strong>Columnas requeridas:</strong><span>producto <small>nombre del producto</small></span><span>cantidad <small>unidades vendidas</small></span><span>precio <small>precio unitario</small></span></div></section>; }

function Dashboard({ result, onReset, jobId, copied, onCopy }: { result: ProcessingResult; onReset: () => void; jobId: string | null; copied: boolean; onCopy: () => void }) { return <section className="dashboard"><div className="dashboard-heading"><div><div className="eyebrow left"><Check size={15} /> ANÁLISIS COMPLETADO</div><h1>Decisiones claras<br /><em>a partir de tus datos.</em></h1><p>{result.FileName} <span>·</span> Procesado el {new Date(result.ProcessedAt).toLocaleString('es-ES')}</p></div><button className="secondary" onClick={onReset}><RotateCcw size={16} /> Analizar otro archivo</button></div><div className="job-meta"><span>ID DEL TRABAJO</span><code>{jobId || result.JobId}</code><button aria-label="Copiar ID del trabajo" onClick={onCopy}>{copied ? <Check size={14} /> : <Clipboard size={14} />}</button>{copied && <small>Copiado</small>}</div><div className="metric-grid"><Metric label="Ingresos totales" value={numberFormat.format(result.TotalRevenue)} icon={<BarChart3 />} /><Metric label="Unidades vendidas" value={numberFormat.format(result.TotalUnits)} icon={<Activity />} /><Metric label="Registros procesados" value={numberFormat.format(result.TotalRecords)} icon={<FileSpreadsheet />} /><Metric label="Producto más vendido" value={result.TopSellingProduct} icon={<Trophy />} /></div><div className="highlight"><span>Producto con mayores ingresos</span><strong>{result.HighestRevenueProduct}</strong><span className="highlight-note">Principal contribuyente de ingresos</span></div><div className="charts"><Chart title="Unidades vendidas por producto" data={result.Products} value="Quantity" /><Chart title="Ingresos por producto" data={result.Products} value="Revenue" /></div></section>; }

function Metric({ label, value, icon }: { label: string; value: string; icon: React.ReactNode }) { return <div className="metric-card"><div className="metric-icon">{icon}</div><span>{label}</span><strong>{value}</strong></div>; }
function Chart({ title, data, value }: { title: string; data: ProductMetric[]; value: 'Quantity' | 'Revenue' }) { const sorted = [...data].sort((a, b) => b[value] - a[value]); const max = sorted[0]?.[value] || 1; return <div className="chart-card"><div className="chart-title"><h3>{title}</h3><span>{value === 'Revenue' ? 'Ingresos' : 'Unidades'}</span></div>{sorted.map(item => <div className="bar-row" key={item.Product}><div className="bar-label"><span>{item.Product}</span><strong>{numberFormat.format(item[value])}</strong></div><div className="bar-track"><div className={`bar-fill ${value === 'Revenue' ? 'revenue' : ''}`} style={{ width: `${Math.max((item[value] / max) * 100, 4)}%` }} /></div></div>)}</div>; }

export default App;
