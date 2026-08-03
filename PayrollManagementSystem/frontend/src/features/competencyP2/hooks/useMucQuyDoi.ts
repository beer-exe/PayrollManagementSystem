import { useState, useCallback } from "react";

import { mucQuyDoiApi } from "../api/mucQuyDoiApi";
import { MucQuyDoiDto, CreateMucQuyDoiDto } from "../types/mucQuyDoi.types";

export const useMucQuyDoi = () => {
  const [data, setData] = useState<MucQuyDoiDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const fetchQuyDoi = useCallback(async () => {
    setLoading(true);
    try {
      const res = await mucQuyDoiApi.getAll();
      if (res.succeeded) {
        setData(res.data);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi tải dữ liệu cấu hình P2", type: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  const createQuyDoi = async (payload: CreateMucQuyDoiDto) => {
    try {
      const res = await mucQuyDoiApi.create(payload);
      if (res.succeeded) {
        setToast({ message: "Thêm mới thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi lưu cấu hình P2", type: 'error' });
      return false;
    }
  };

  const updateQuyDoi = async (id: string, payload: CreateMucQuyDoiDto) => {
    try {
      const res = await mucQuyDoiApi.update(id, payload);
      if (res.succeeded) {
        setToast({ message: "Cập nhật thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi cập nhật cấu hình P2", type: 'error' });
      return false;
    }
  };

  const deleteQuyDoi = async (id: string) => {
    try {
      const res = await mucQuyDoiApi.delete(id);
      if (res.succeeded) {
        setToast({ message: "Xóa thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi xóa cấu hình P2", type: 'error' });
      return false;
    }
  };

  return {
    data,
    loading,
    fetchQuyDoi,
    createQuyDoi,
    updateQuyDoi,
    deleteQuyDoi,
    toast,
    setToast,
  };
};
