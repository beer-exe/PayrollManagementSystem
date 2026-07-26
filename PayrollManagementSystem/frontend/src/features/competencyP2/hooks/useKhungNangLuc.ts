import { useState, useCallback } from "react";

import { khungNangLucApi } from "../api/khungNangLucApi";
import { KhungNangLucDto, CreateKhungNangLucCommand, UpdateKhungNangLucCommand } from "../types/khungNangLuc.types";

export const useKhungNangLuc = () => {
  const [data, setData] = useState<KhungNangLucDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const fetchByChucVu = useCallback(async (idChucVu: string) => {
    if (!idChucVu) return;
    setLoading(true);
    try {
      const res = await khungNangLucApi.getByChucVu(idChucVu);
      if (res.succeeded) {
        setData(res.data);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi tải khung năng lực", type: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  const createCriteria = async (payload: CreateKhungNangLucCommand) => {
    try {
      const res = await khungNangLucApi.create(payload);
      if (res.succeeded) {
        setToast({ message: "Thêm tiêu chí thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi thêm tiêu chí", type: 'error' });
      return false;
    }
  };

  const updateCriteria = async (id: string, payload: UpdateKhungNangLucCommand) => {
    try {
      const res = await khungNangLucApi.update(id, payload);
      if (res.succeeded) {
        setToast({ message: "Cập nhật thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi cập nhật", type: 'error' });
      return false;
    }
  };

  const deleteCriteria = async (id: string) => {
    try {
      const res = await khungNangLucApi.delete(id);
      if (res.succeeded) {
        setToast({ message: "Xóa tiêu chí thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi xóa", type: 'error' });
      return false;
    }
  };

  return {
    data,
    loading,
    fetchByChucVu,
    createCriteria,
    updateCriteria,
    deleteCriteria,
    toast,
    setToast,
  };
};
