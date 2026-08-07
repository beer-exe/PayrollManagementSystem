import { useState, useCallback, useEffect } from 'react';
import { MyPayrollDto } from '../types/myPayroll.types';
import { personalPayrollApi } from '../api/personalPayrollApi';

export const useMyPayroll = (initialYear: number) => {
  const [data, setData] = useState<MyPayrollDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [year, setYear] = useState(initialYear);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const showToast = useCallback((message: string, type: 'success' | 'error') => {
    setToast({ message, type });
  }, []);

  const fetchMyPayroll = useCallback(async (selectedYear: number) => {
    setLoading(true);
    setError(null);
    try {
      const result = await personalPayrollApi.getMyPayroll(selectedYear);
      const finalData = Array.isArray(result) ? result : (result as any).data || [];
      setData(finalData);
    } catch (err: any) {
      const msg = err.response?.data?.message || 'Lỗi khi tải bảng lương cá nhân';
      setError(msg);
      showToast(msg, 'error');
      setData([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchMyPayroll(year);
  }, [year, fetchMyPayroll]);

  return {
    data,
    loading,
    error,
    year,
    setYear,
    refetch: () => fetchMyPayroll(year),
    toast,
    setToast
  };
};
