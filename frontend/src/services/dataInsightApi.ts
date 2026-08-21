import type { CreateJobResponse, CreateUploadResponse, ProcessingResult } from '../types/dataInsight';
const base = import.meta.env.VITE_API_BASE_URL || 'https://2854n43gfl.execute-api.us-east-1.amazonaws.com/Prod';
async function request<T>(path: string, init: RequestInit): Promise<T> {
  const headers = new Headers(init.headers);
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json');
  const response = await fetch(`${base}${path}`, { ...init, headers });
  const body = await response.text();
  if (!response.ok) {
    let message = `Request failed (${response.status})`;
    try { message = JSON.parse(body).message || message; } catch { /* use status message */ }
    throw new Error(message);
  }
  return body ? JSON.parse(body) as T : {} as T;
}
export const createUploadUrl = (fileName: string, contentType: string) => request<CreateUploadResponse>('/uploads', { method: 'POST', body: JSON.stringify({ fileName, contentType }) });
export async function uploadFileToS3(url: string, file: File, contentType: string) {
  const response = await fetch(url, { method: 'PUT', headers: { 'Content-Type': contentType }, body: file });
  if (!response.ok) throw new Error(`S3 upload failed (${response.status})`);
}
export const createJob = (fileName: string, contentType: string, objectKey: string) => request<CreateJobResponse>('/jobs', { method: 'POST', body: JSON.stringify({ fileName, contentType, objectKey }) });
export async function getJobResult(jobId: string) {
  const raw = await request<Record<string, unknown>>(`/jobs/${encodeURIComponent(jobId)}`, { method: 'GET' });
  return {
    ...raw,
    JobId: raw.JobId ?? raw.jobId ?? jobId,
    Status: raw.Status ?? raw.status ?? ''
  } as unknown as ProcessingResult;
}
