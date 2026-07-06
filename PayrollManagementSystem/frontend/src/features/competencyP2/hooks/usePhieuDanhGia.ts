import { useState, useCallback } from "react";
import { message } from "antd";
import { phieuDanhGiaApi } from "../api/phieuDanhGiaApi";
import { PhieuDanhGiaDto, GenerateMyPhieuDanhGiaCommand, SubmitTuDanhGiaCommand, SubmitManagerEvaluationCommand } from "../types/phieuDanhGia.types";

export const usePhieuDanhGia = () => {
  const [data, setData] = useState<PhieuDanhGiaDto[]>([]);
  const [detail, setDetail] = useState<PhieuDanhGiaDto | null>(null);
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
      message.error(error.response?.data?.Message || "Lỗi khi tải danh sách phiếu đánh giá");
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
      message.error(error.response?.data?.Message || "Lỗi khi tải chi tiết phiếu");
    } finally {
      setLoading(false);
    }
  }, []);

  const generate = async (payload: GenerateMyPhieuDanhGiaCommand) => {
    try {
      const res = await phieuDanhGiaApi.generate(payload);
      if (res.succeeded) {
        message.success("Đã tạo phiếu đánh giá thành công.");
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi sinh phiếu");
      return false;
    }
  };

  const submitTuDanhGia = async (payload: SubmitTuDanhGiaCommand) => {
    try {
      const res = await phieuDanhGiaApi.submitTuDanhGia(payload);
      if (res.succeeded) {
        message.success(payload.isSubmit ? "Gửi đánh giá thành công" : "Lưu nháp thành công");
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi lưu đánh giá");
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
      message.error(error.response?.data?.Message || "Lỗi khi tải danh sách phiếu cần duyệt");
    } finally {
      setLoading(false);
    }
  }, []);

  const submitManagerEvaluation = async (payload: SubmitManagerEvaluationCommand) => {
    try {
      const res = await phieuDanhGiaApi.managerSubmit(payload);
      if (res.succeeded) {
        message.success(payload.isSubmit ? "Chốt phiếu thành công" : "Lưu nháp thành công");
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      console.error(error);
      message.error(error.response?.data?.Message || "Lỗi khi duyệt phiếu");
      return false;
    }
  };

  return {
    data,
    detail,
    loading,
    fetchMyEvaluations,
    fetchById,
    generate,
    submitTuDanhGia,
    fetchManagerEvaluations,
    submitManagerEvaluation
  };
};
