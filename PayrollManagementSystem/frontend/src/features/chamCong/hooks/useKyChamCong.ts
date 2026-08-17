import { useState, useCallback } from 'react';
import { kyChamCongApi, KyChamCongDto } from '../api/kyChamCongApi';

export const useKyChamCong = () => {
  const [kyChamCong, setKyChamCong] = useState<KyChamCongDto | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchKyChamCong = useCallback(async (nam: number, thang: number) => {
    try {
      setLoading(true);
      setError(null);
      const res = await kyChamCongApi.getKyChamCong(nam, thang);
      if (res.succeeded) {
        setKyChamCong(res.data);
      } else {
        setError(res.message || 'Lỗi khi lấy thông tin kỳ chấm công');
      }
    } catch (err: any) {
      const msg = err.response?.data?.Message || 'Lỗi khi lấy thông tin kỳ chấm công';
      setError(msg);
    } finally {
      setLoading(false);
    }
  }, []);

  const chotCong = async (nam: number, thang: number): Promise<{ success: boolean; message?: string }> => {
    try {
      setLoading(true);
      setError(null);
      const res = await kyChamCongApi.chotCong(nam, thang);
      if (res.succeeded) {
        await fetchKyChamCong(nam, thang);
        return { success: true };
      } else {
        const msg = res.message || 'Lỗi khi chốt công';
        setError(msg);
        return { success: false, message: msg };
      }
    } catch (err: any) {
      const msg = err.response?.data?.Message || 'Lỗi khi chốt công';
      setError(msg);
      return { success: false, message: msg };
    } finally {
      setLoading(false);
    }
  };

  const moChotCong = async (nam: number, thang: number): Promise<{ success: boolean; message?: string }> => {
    try {
      setLoading(true);
      setError(null);
      const res = await kyChamCongApi.moChotCong(nam, thang);
      if (res.succeeded) {
        await fetchKyChamCong(nam, thang);
        return { success: true };
      } else {
        const msg = res.message || 'Lỗi khi mở chốt công';
        setError(msg);
        return { success: false, message: msg };
      }
    } catch (err: any) {
      const msg = err.response?.data?.Message || 'Lỗi khi mở chốt công';
      setError(msg);
      return { success: false, message: msg };
    } finally {
      setLoading(false);
    }
  };

  return {
    kyChamCong,
    loading,
    error,
    fetchKyChamCong,
    chotCong,
    moChotCong,
  };
};
