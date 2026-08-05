import { useState, useCallback } from 'react';
import type { SystemLogDto, SystemLogFilter, PagedResult } from '../types';
import { systemLogsApi } from '../api/systemLogsApi';

const DEFAULT_FILTER: SystemLogFilter = {
  pageNumber: 1,
  pageSize: 50,
};

export function useSystemLogs() {
  const [logs, setLogs] = useState<SystemLogDto[]>([]);
  const [filter, setFilter] = useState<SystemLogFilter>(DEFAULT_FILTER);
  const [pagedResult, setPagedResult] = useState<Omit<PagedResult<SystemLogDto>, 'data'> | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchLogs = useCallback(async (overrideFilter?: SystemLogFilter) => {
    const activeFilter = overrideFilter ?? filter;
    setLoading(true);
    setError(null);
    try {
      const result = await systemLogsApi.getLogs(activeFilter);
      setLogs(result.data ?? []);
      setPagedResult({
        pageNumber: result.pageNumber,
        pageSize: result.pageSize,
        totalPages: result.totalPages,
        totalRecords: result.totalRecords,
        succeeded: result.succeeded,
      });
    } catch {
      setError('Không thể tải danh sách log. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  }, [filter]);

  const applyFilter = useCallback((newFilter: Partial<SystemLogFilter>) => {
    const updated = { ...filter, ...newFilter, pageNumber: 1 };
    setFilter(updated);
    fetchLogs(updated);
  }, [filter, fetchLogs]);

  const changePage = useCallback((page: number) => {
    const updated = { ...filter, pageNumber: page };
    setFilter(updated);
    fetchLogs(updated);
  }, [filter, fetchLogs]);

  const resetFilter = useCallback(() => {
    setFilter(DEFAULT_FILTER);
    fetchLogs(DEFAULT_FILTER);
  }, [fetchLogs]);

  return { logs, filter, pagedResult, loading, error, fetchLogs, applyFilter, changePage, resetFilter };
}
