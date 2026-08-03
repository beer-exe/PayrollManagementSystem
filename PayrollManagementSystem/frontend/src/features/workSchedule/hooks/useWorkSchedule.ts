import { useState, useCallback } from 'react';
import { workScheduleApi } from '../api/workScheduleApi';
import type { LichLamViecDto, ChiTietLichLamViecDto, CreateLichLamViecRequest } from '../types/workSchedule.types';

export const useWorkSchedule = () => {
  const [lichList, setLichList] = useState<LichLamViecDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const showToast = useCallback((message: string, type: 'success' | 'error') => {
    setToast({ message, type });
  }, []);

  const fetchAll = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const res = await workScheduleApi.getAll();
      setLichList(res.data ?? []);
    } catch (err: any) {
      const msg = err?.response?.data?.message ?? 'Có lỗi xảy ra khi tải dữ liệu.';
      setError(msg);
      showToast(msg, 'error');
    } finally {
      setIsLoading(false);
    }
  }, [showToast]);

  const create = useCallback(async (data: CreateLichLamViecRequest) => {
    setIsCreating(true);
    setError(null);
    try {
      const res = await workScheduleApi.create(data);
      showToast(res.message, 'success');
      await fetchAll();
      return true;
    } catch (err: any) {
      const msg = err?.response?.data?.message ?? `Không thể tạo lịch làm việc năm ${data.nam}.`;
      setError(msg);
      showToast(msg, 'error');
      return false;
    } finally {
      setIsCreating(false);
    }
  }, [fetchAll, showToast]);

  const remove = useCallback(async (id: string, nam: number) => {
    setIsLoading(true);
    setError(null);
    try {
      const res = await workScheduleApi.delete(id);
      showToast(res.message, 'success');
      await fetchAll();
      return true;
    } catch (err: any) {
      const msg = err?.response?.data?.message ?? `Không thể xóa lịch làm việc năm ${nam}.`;
      setError(msg);
      showToast(msg, 'error');
      return false;
    } finally {
      setIsLoading(false);
    }
  }, [fetchAll, showToast]);

  const clearMessages = () => {
    setError(null);
    setToast(null);
  };

  return { 
    lichList, 
    isLoading, 
    isCreating, 
    error, 
    fetchAll, 
    create, 
    remove,
    clearMessages,
    toast,
    setToast,
  };
};

export const useChiTietLich = () => {
  const [chiTiets, setChiTiets] = useState<ChiTietLichLamViecDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [totalRecords, setTotalRecords] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  const fetch = useCallback(async (idLich: string, thang: number, pageNumber: number = 1) => {
    setIsLoading(true);
    try {
      const res = await workScheduleApi.getChiTiet(idLich, thang, pageNumber, 31);
      setChiTiets(res.data ?? []);
      setTotalRecords(res.totalRecords ?? 0);
      setTotalPages(res.totalPages ?? 1);
    } catch {
      setChiTiets([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  return { chiTiets, setChiTiets, isLoading, totalRecords, totalPages, fetch };
};
