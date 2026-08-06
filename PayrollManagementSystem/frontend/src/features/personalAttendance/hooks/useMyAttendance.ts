import { useState, useCallback } from 'react';
import { ChamCongDto } from '../../chamCong/types/chamCong.types';
import { personalAttendanceApi } from '../api/personalAttendanceApi';

export const useMyAttendance = () => {
  const [attendanceList, setAttendanceList] = useState<ChamCongDto[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchAttendance = useCallback(async (thang: number, nam: number) => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await personalAttendanceApi.getMyAttendance(thang, nam);
      // Data might be wrapped or unwrapped depending on interceptor
      const finalData = Array.isArray(data) ? data : (data as any).data || [];
      setAttendanceList(finalData);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Có lỗi xảy ra khi tải dữ liệu chấm công cá nhân.');
      setAttendanceList([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    attendanceList,
    isLoading,
    error,
    fetchAttendance
  };
};
