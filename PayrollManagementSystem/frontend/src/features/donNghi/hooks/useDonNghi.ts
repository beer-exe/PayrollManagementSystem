import { useState, useCallback } from 'react';
import { donNghiApi } from '../api/donNghiApi';
import type { DonNghiDto, NgayPhepDto, CreateDonNghiRequest, TuChoiRequest, UpdateNgayPhepRequest } from '../types/donNghi.types';

export const useDonNghi = () => {
  const [list, setList] = useState<DonNghiDto[]>([]);
  const [ngayPhepList, setNgayPhepList] = useState<NgayPhepDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchList = useCallback(async (params: {
    thang?: number; nam?: number; cccd?: string; trangThai?: string; idPhongBan?: string;
  }) => {
    setLoading(true); setError(null);
    try {
      const res = await donNghiApi.getList(params);
      setList(res.data ?? []);
    } catch {
      setError('Không thể tải danh sách đơn nghỉ.');
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchNgayPhep = useCallback(async (nam: number, idPhongBan?: string) => {
    setLoading(true); setError(null);
    try {
      const res = await donNghiApi.getNgayPhep(nam, idPhongBan);
      setNgayPhepList(res.data ?? []);
    } catch {
      setError('Không thể tải danh sách ngày phép.');
    } finally {
      setLoading(false);
    }
  }, []);

  const createDonNghi = useCallback(async (data: CreateDonNghiRequest): Promise<string | null> => {
    try {
      await donNghiApi.create(data);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string; Message?: string } } };
      return err?.response?.data?.Message ?? err?.response?.data?.message ?? 'Tạo đơn nghỉ thất bại.';
    }
  }, []);

  const duyetDonNghi = useCallback(async (id: string): Promise<string | null> => {
    try {
      await donNghiApi.duyet(id);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string; Message?: string } } };
      return err?.response?.data?.Message ?? err?.response?.data?.message ?? 'Duyệt đơn thất bại.';
    }
  }, []);

  const tuChoiDonNghi = useCallback(async (id: string, body: TuChoiRequest): Promise<string | null> => {
    try {
      await donNghiApi.tuChoi(id, body);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string; Message?: string } } };
      return err?.response?.data?.Message ?? err?.response?.data?.message ?? 'Từ chối đơn thất bại.';
    }
  }, []);

  const deleteDonNghi = useCallback(async (id: string): Promise<string | null> => {
    try {
      await donNghiApi.delete(id);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string } } };
      return err?.response?.data?.message ?? 'Xóa đơn thất bại.';
    }
  }, []);

  const updateNgayPhep = useCallback(async (data: UpdateNgayPhepRequest): Promise<string | null> => {
    try {
      await donNghiApi.updateNgayPhep(data);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string; Message?: string } } };
      return err?.response?.data?.Message ?? err?.response?.data?.message ?? 'Cập nhật phép thất bại.';
    }
  }, []);

  const huyDonNghiDaDuyet = useCallback(async (id: string): Promise<string | null> => {
    try {
      await donNghiApi.huyDaDuyet(id);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string; Message?: string } } };
      return err?.response?.data?.Message ?? err?.response?.data?.message ?? 'Hủy đơn thất bại.';
    }
  }, []);

  return {
    list, ngayPhepList, loading, error,
    fetchList, fetchNgayPhep,
    createDonNghi, duyetDonNghi, tuChoiDonNghi, deleteDonNghi, updateNgayPhep, huyDonNghiDaDuyet
  };
};
