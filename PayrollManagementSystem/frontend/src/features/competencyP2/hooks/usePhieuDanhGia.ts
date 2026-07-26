import { useState, useCallback } from "react";

import { phieuDanhGiaApi } from "../api/phieuDanhGiaApi";
import { PhieuDanhGiaDto, GenerateMyPhieuDanhGiaCommand, SubmitTuDanhGiaCommand, SubmitManagerEvaluationCommand, ChiTietDanhGiaDto } from "../types/phieuDanhGia.types";

export const usePhieuDanhGia = () => {
  const [data, setData] = useState<PhieuDanhGiaDto[]>([]);
  const [detail, setDetail] = useState<any>(null);
  const [selectedPhieu] = useState<ChiTietDanhGiaDto | null>(null);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);
  const [loading, setLoading] = useState(false);

  const fetchMyEvaluations = useCallback(async () => {
    setLoading(true);
    try {
      const res = await phieuDanhGiaApi.getMyEvaluations();
      if (res.succeeded) {
        setData(res.data);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi tải danh sách phiếu đánh giá", type: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchById = useCallback(async (id: string) => {
    setLoading(true);
    try {
      const res = await phieuDanhGiaApi.getById(id);
      if (res.succeeded) {
        setDetail(res.data);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi tải chi tiết phiếu", type: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  const generate = async (payload: GenerateMyPhieuDanhGiaCommand) => {
    try {
      const res = await phieuDanhGiaApi.generate(payload);
      if (res.succeeded) {
        setToast({ message: "Đã tạo phiếu đánh giá thành công.", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi sinh phiếu", type: 'error' });
      return false;
    }
  };

  const submitTuDanhGia = async (payload: SubmitTuDanhGiaCommand) => {
    try {
      const res = await phieuDanhGiaApi.submitTuDanhGia(payload);
      if (res.succeeded) {
        setToast({ message: payload.isSubmit ? "Gửi đánh giá thành công" : "Lưu nháp thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi lưu đánh giá", type: 'error' });
      return false;
    }
  };

  const fetchManagerEvaluations = useCallback(async () => {
    setLoading(true);
    try {
      const res = await phieuDanhGiaApi.getManagerEvaluations();
      if (res.succeeded) {
        setData(res.data);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi tải danh sách phiếu cần duyệt", type: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  const submitManagerEvaluation = async (payload: SubmitManagerEvaluationCommand) => {
    try {
      const res = await phieuDanhGiaApi.managerSubmit(payload);
      if (res.succeeded) {
        setToast({ message: payload.isSubmit ? "Chốt phiếu thành công" : "Lưu nháp thành công", type: 'success' });
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      setToast({ message: error.response?.data?.Message || "Lỗi khi duyệt phiếu", type: 'error' });
      return false;
    }
  };

  return {
    data,
    detail,
    selectedPhieu,
    loading,
    fetchMyEvaluations,
    fetchById,
    generate,
    submitTuDanhGia,
    fetchManagerEvaluations,
    submitManagerEvaluation,
    toast,
    setToast
  };
};
