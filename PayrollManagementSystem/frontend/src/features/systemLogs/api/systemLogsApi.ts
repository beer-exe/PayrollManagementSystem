import axiosClient from '@/services/api/axiosClient';
import type { PagedResult, SystemLogDto, SystemLogFilter } from '../types';

export const systemLogsApi = {
  getLogs: async (filter: SystemLogFilter): Promise<PagedResult<SystemLogDto>> => {
    const params = new URLSearchParams();
    if (filter.level) params.set('level', filter.level);
    if (filter.fromDate) params.set('fromDate', filter.fromDate);
    if (filter.toDate) params.set('toDate', filter.toDate);
    if (filter.keyword) params.set('keyword', filter.keyword);
    if (filter.sortBy) params.set('sortBy', filter.sortBy);
    if (filter.sortDirection) params.set('sortDirection', filter.sortDirection);
    params.set('pageNumber', String(filter.pageNumber));
    params.set('pageSize', String(filter.pageSize));

    return axiosClient.get(`/system-logs?${params.toString()}`);
  },
};
