import { useCallback, useEffect, useRef, useState } from 'react';
import { getJobResult } from '../services/dataInsightApi';
import type { ProcessingResult } from '../types/dataInsight';

const POLL_INTERVAL = 2000;
const POLL_TIMEOUT = 60000;

export function useJobPolling(jobId: string | null, enabled: boolean) {
  const [result, setResult] = useState<ProcessingResult | null>(null);
  const [error, setError] = useState('');
  const [timedOut, setTimedOut] = useState(false);
  const startedAt = useRef(0);
  const timer = useRef<ReturnType<typeof setTimeout> | null>(null);

  const clearTimer = useCallback(() => {
    if (timer.current) {
      clearTimeout(timer.current);
      timer.current = null;
    }
  }, []);

  const poll = useCallback(async () => {
    if (!jobId || !enabled) return;
    if (Date.now() - startedAt.current >= POLL_TIMEOUT) {
      setTimedOut(true);
      return;
    }
    try {
      const response = await getJobResult(jobId);
      if (response.Status.toLowerCase() === 'completed') {
        setResult(response);
        clearTimer();
        return;
      }
      timer.current = setTimeout(poll, POLL_INTERVAL);
    } catch (pollError) {
      setError(pollError instanceof Error ? pollError.message : 'Unable to check processing status.');
    }
  }, [clearTimer, enabled, jobId]);

  const retry = useCallback(() => {
    setError('');
    setTimedOut(false);
    startedAt.current = Date.now();
    clearTimer();
    void poll();
  }, [clearTimer, poll]);

  useEffect(() => {
    clearTimer();
    if (!jobId) {
      setResult(null);
      setError('');
      setTimedOut(false);
      return clearTimer;
    }
    if (enabled) {
      setResult(null);
      setError('');
      setTimedOut(false);
      startedAt.current = Date.now();
      void poll();
    }
    return clearTimer;
  }, [clearTimer, enabled, jobId, poll]);

  return { result, error, timedOut, retry };
}
