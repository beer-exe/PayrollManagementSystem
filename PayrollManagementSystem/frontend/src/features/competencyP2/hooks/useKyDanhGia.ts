import { useState, useCallback } from "react";
import { message } from "antd";
import { kyDanhGiaApi } from "../api/kyDanhGiaApi";
import { KyDanhGiaDto, CreateKyDanhGiaDto } from "../types/kyDanhGia.types";

export const useKyDanhGia = () => {
  const [data, setData] = useState<KyDanhGiaDto[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchKyDanhGia = useCallback(async () => {
    setLoading(true);
    try {
      const res = await kyDanhGiaApi.getAll();
      if (res.succeeded) {
        setData(res.data);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi tải dữ liệu kỳ đánh giá");
    } finally {
      setLoading(false);
    }
  }, []);

  const createKyDanhGia = async (payload: CreateKyDanhGiaDto) => {
    try {
      const res = await kyDanhGiaApi.create(payload);
      if (res.succeeded) {
        message.success("Thêm mới thành công");
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi tạo kỳ đánh giá");
      return false;
    }
  };

  return {
    data,
    loading,
    fetchKyDanhGia,
    createKyDanhGia,
  };
};
