import { useState, useCallback } from 'react';
import { workScheduleApi } from '../api/workScheduleApi';
import type { LichLamViecDto, ChiTietLichLamViecDto, CreateLichLamViecRequest } from '../types/workSchedule.types';

export const useWorkSchedule = () => {
  const [lichList, setLichList] = useState<LichLamViecDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  const fetchAll = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const res = await workScheduleApi.getAll();
      setLichList(res.data ?? []);
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'Có lỗi xảy ra khi tải dữ liệu.');
    } finally {
      setIsLoading(false);
    }
  }, []);

  const create = useCallback(async (data: CreateLichLamViecRequest) => {
    setIsCreating(true);
    setError(null);
    setSuccessMsg(null);
    try {
      const res = await workScheduleApi.create(data);
      setSuccessMsg(res.message);
      await fetchAll();
      return true;
    } catch (err: any) {
      setError(err?.response?.data?.message ?? `Không thể tạo lịch làm việc năm ${data.nam}.`);
      return false;
    } finally {
      setIsCreating(false);
    }
  }, [fetchAll]);

  const remove = useCallback(async (id: string, nam: number) => {
    setIsLoading(true);
    setError(null);
    setSuccessMsg(null);
    try {
      const res = await workScheduleApi.delete(id);
      setSuccessMsg(res.message);
      await fetchAll();
      return true;
    } catch (err: any) {
      setError(err?.response?.data?.message ?? `Không thể xóa lịch làm việc năm ${nam}.`);
      return false;
    } finally {
      setIsLoading(false);
    }
  }, [fetchAll]);

  const clearMessages = () => {
    setError(null);
    setSuccessMsg(null);
  };

  return { lichList, isLoading, isCreating, error, successMsg, fetchAll, create, remove, clearMessages };
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
