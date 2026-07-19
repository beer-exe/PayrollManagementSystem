import axiosClient from '@/services/api/axiosClient';
import type { LichLamViecDto, ChiTietLichLamViecDto, CreateLichLamViecRequest } from '../types/workSchedule.types';

const BASE_URL = '/lich-lam-viec';

export const workScheduleApi = {
  getAll: (): Promise<{ data: LichLamViecDto[]; succeeded: boolean; message: string }> =>
    axiosClient.get(BASE_URL),

  create: (data: CreateLichLamViecRequest): Promise<{ data: string; succeeded: boolean; message: string }> =>
    axiosClient.post(BASE_URL, data),

  delete: (id: string): Promise<{ data: boolean; succeeded: boolean; message: string }> =>
    axiosClient.delete(`${BASE_URL}/${id}`),

  getChiTiet: (
    id: string,
    thang: number,
    pageNumber: number = 1,
    pageSize: number = 31
  ): Promise<{
    data: ChiTietLichLamViecDto[];
    succeeded: boolean;
    message: string;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    totalRecords: number;
  }> =>
    axiosClient.get(`${BASE_URL}/${id}/chi-tiet`, {
      params: { thang, pageNumber, pageSize },
    }),

  updateChiTiet: (idChiTiet: string, loaiNgay: string, tenNgayNghi?: string): Promise<{ data: boolean; succeeded: boolean; message: string }> =>
    axiosClient.put(`${BASE_URL}/chi-tiet`, { idChiTiet, loaiNgay, tenNgayNghi }),
};
