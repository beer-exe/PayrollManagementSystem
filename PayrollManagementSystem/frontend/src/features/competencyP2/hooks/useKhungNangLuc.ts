import { useState, useCallback } from "react";
import { message } from "antd";
import { khungNangLucApi } from "../api/khungNangLucApi";
import { KhungNangLucDto, CreateKhungNangLucCommand, UpdateKhungNangLucCommand } from "../types/khungNangLuc.types";

export const useKhungNangLuc = () => {
  const [data, setData] = useState<KhungNangLucDto[]>([]);
  const [loading, setLoading] = useState(false);

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
      message.error(error.response?.data?.Message || "Lỗi khi tải khung năng lực");
    } finally {
      setLoading(false);
    }
  }, []);

  const createCriteria = async (payload: CreateKhungNangLucCommand) => {
    try {
      const res = await khungNangLucApi.create(payload);
      if (res.succeeded) {
        message.success("Thêm tiêu chí thành công");
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi thêm tiêu chí");
      return false;
    }
  };

  const updateCriteria = async (id: string, payload: UpdateKhungNangLucCommand) => {
    try {
      const res = await khungNangLucApi.update(id, payload);
      if (res.succeeded) {
        message.success("Cập nhật thành công");
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi cập nhật");
      return false;
    }
  };

  const deleteCriteria = async (id: string) => {
    try {
      const res = await khungNangLucApi.delete(id);
      if (res.succeeded) {
        message.success("Xóa tiêu chí thành công");
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi xóa");
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
  };
};
