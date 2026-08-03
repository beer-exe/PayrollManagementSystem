import { useState, useCallback } from "react";

import { kyDanhGiaApi } from "../api/kyDanhGiaApi";
import { KyDanhGiaDto, CreateKyDanhGiaDto } from "../types/kyDanhGia.types";

export const useKyDanhGia = () => {
  const [data, setData] = useState<KyDanhGiaDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const fetchKyDanhGia = useCallback(async () => {
    setLoading(true);
    try {
      const res = await kyDanhGiaApi.getAll();
      if (res.succeeded) {
        setData(res.data);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi tải dữ liệu kỳ đánh giá", type: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  const createKyDanhGia = async (payload: CreateKyDanhGiaDto) => {
    try {
      const res = await kyDanhGiaApi.create(payload);
      if (res.succeeded) {
        setToast({ message: "Thêm mới thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi tạo kỳ đánh giá", type: 'error' });
      return false;
    }
  };

  return {
    data,
    loading,
    fetchKyDanhGia,
    createKyDanhGia,
    toast,
    setToast,
  };
};
