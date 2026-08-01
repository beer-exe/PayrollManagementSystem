export interface SystemLogDto {
  id: number;
  raiseDate: string;
  level: string;
  message: string | null;
  exception: string | null;
  properties: string | null;
}

export interface SystemLogFilter {
  level?: string;
  fromDate?: string;
  toDate?: string;
  keyword?: string;
  pageNumber: number;
  pageSize: number;
}

export interface PagedResult<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalRecords: number;
  succeeded: boolean;
}

export const LOG_LEVELS = ['Information', 'Warning', 'Error', 'Fatal', 'Debug'] as const;
export type LogLevel = typeof LOG_LEVELS[number];

export const LOG_LEVEL_COLORS: Record<string, { bg: string; text: string; label: string }> = {
  Information: { bg: '#dbeafe', text: '#1d4ed8', label: 'INF' },
  Warning:     { bg: '#fef3c7', text: '#b45309', label: 'WRN' },
  Error:       { bg: '#fee2e2', text: '#b91c1c', label: 'ERR' },
  Fatal:       { bg: '#ede9fe', text: '#6d28d9', label: 'FTL' },
  Debug:       { bg: '#f0fdf4', text: '#15803d', label: 'DBG' },
};
