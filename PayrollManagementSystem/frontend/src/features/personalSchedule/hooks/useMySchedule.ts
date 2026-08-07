import { useState, useCallback } from 'react';
import { personalScheduleApi } from '../api/personalScheduleApi';
import { MyScheduleDayDto } from '../types/personalSchedule.types';

export const useMySchedule = () => {
  const [schedule, setSchedule] = useState<MyScheduleDayDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const showToast = useCallback((message: string, type: 'success' | 'error') => {
    setToast({ message, type });
  }, []);

  const fetchSchedule = useCallback(async (thang: number, nam: number) => {
    setIsLoading(true);
    setError(null);
    try {
      const res = await personalScheduleApi.getMySchedule(thang, nam);
      setSchedule(res.data ?? []);
    } catch (err: any) {
      const msg = err?.response?.data?.message ?? 'Có lỗi xảy ra khi tải lịch làm việc.';
      setError(msg);
      showToast(msg, 'error');
      setSchedule([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    schedule,
    isLoading,
    error,
    fetchSchedule,
    toast,
    setToast
  };
};
