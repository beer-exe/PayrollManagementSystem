import { useState, useCallback } from 'react';
import { chamCongApi } from '../api/chamCongApi';
import type {
  ChamCongDto,
  ChamCongSummaryDto,
  CreateChamCongRequest,
  UpdateChamCongRequest,
  ImportChamCongResultDto,
} from '../types/chamCong.types';

export const useChamCong = () => {
  const [list, setList] = useState<ChamCongDto[]>([]);
  const [summary, setSummary] = useState<ChamCongSummaryDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchList = useCallback(async (thang: number, nam: number, cccd?: string, idPhongBan?: string) => {
    setLoading(true);
    setError(null);
    try {
      const res = await chamCongApi.getList(thang, nam, cccd, idPhongBan);
      if (res.succeeded) setList(res.data);
      else setError(res.message);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Lỗi tải dữ liệu chấm công.');
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchSummary = useCallback(async (thang: number, nam: number, idPhongBan?: string) => {
    setLoading(true);
    setError(null);
    try {
      const res = await chamCongApi.getSummary(thang, nam, idPhongBan);
      if (res.succeeded) setSummary(res.data);
      else setError(res.message);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Lỗi tải tổng hợp chấm công.');
    } finally {
      setLoading(false);
    }
  }, []);

  const createChamCong = useCallback(async (data: CreateChamCongRequest): Promise<string | null> => {
    try {
      const res = await chamCongApi.create(data);
      if (res.succeeded) return null;
      return res.message;
    } catch (err: unknown) {
      return err instanceof Error ? err.message : 'Lỗi nhập chấm công.';
    }
  }, []);

  const updateChamCong = useCallback(async (id: string, data: UpdateChamCongRequest): Promise<string | null> => {
    try {
      const res = await chamCongApi.update(id, data);
      if (res.succeeded) return null;
      return res.message;
    } catch (err: unknown) {
      return err instanceof Error ? err.message : 'Lỗi cập nhật chấm công.';
    }
  }, []);

  const deleteChamCong = useCallback(async (id: string): Promise<string | null> => {
    try {
      const res = await chamCongApi.delete(id);
      if (res.succeeded) return null;
      return res.message;
    } catch (err: unknown) {
      return err instanceof Error ? err.message : 'Lỗi xóa bản ghi.';
    }
  }, []);

  const importChamCong = useCallback(async (file: File): Promise<ImportChamCongResultDto | null> => {
    try {
      const res = await chamCongApi.import(file);
      if (res.succeeded) return res.data;
      setError(res.message);
      return null;
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Lỗi import chấm công.');
      return null;
    }
  }, []);

  return {
    list,
    summary,
    loading,
    error,
    fetchList,
    fetchSummary,
    createChamCong,
    updateChamCong,
    deleteChamCong,
    importChamCong,
  };
};
