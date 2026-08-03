import { useState, useCallback } from 'react';
import { myDonNghiApi, type CreateMyDonNghiRequest } from '../api/myDonNghiApi';
import type { DonNghiDto, NgayPhepDto } from '../types/donNghi.types';

export const useMyDonNghi = () => {
  const [list, setList] = useState<DonNghiDto[]>([]);
  const [ngayPhep, setNgayPhep] = useState<NgayPhepDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const showToast = useCallback((type: 'success' | 'error', msg: string) => {
    setToast({ message: msg, type });
  }, []);

  const fetchList = useCallback(async (params: {
    thang?: number;
    nam?: number;
    trangThai?: string;
    loaiNghi?: string;
  }) => {
    setLoading(true);
    try {
      const res = await myDonNghiApi.getMyList(params);
      setList(res.data ?? []);
    } catch {
      showToast('error', 'Không thể tải danh sách đơn nghỉ.');
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchNgayPhep = useCallback(async (nam: number) => {
    try {
      const res = await myDonNghiApi.getMyNgayPhep(nam);
      setNgayPhep(res.data ?? null);
    } catch {
      setNgayPhep(null);
    }
  }, []);

  const createDonNghi = useCallback(async (data: CreateMyDonNghiRequest): Promise<string | null> => {
    try {
      await myDonNghiApi.createMy(data);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { Message?: string; message?: string } } };
      return err?.response?.data?.Message ?? err?.response?.data?.message ?? 'Nộp đơn nghỉ thất bại.';
    }
  }, []);

  const deleteDonNghi = useCallback(async (id: string): Promise<string | null> => {
    try {
      await myDonNghiApi.deleteMy(id);
      return null;
    } catch (e: unknown) {
      const err = e as { response?: { data?: { Message?: string; message?: string } } };
      return err?.response?.data?.Message ?? err?.response?.data?.message ?? 'Hủy đơn nghỉ thất bại.';
    }
  }, []);

  return { list, ngayPhep, loading, fetchList, fetchNgayPhep, createDonNghi, deleteDonNghi, toast, setToast, showToast };
};
