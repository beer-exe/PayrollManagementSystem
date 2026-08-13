import axiosClient from '@/services/api/axiosClient';
import type {
  ChamCongDto,
  ChamCongSummaryDto,
  ImportChamCongResultDto,
  CreateChamCongRequest,
  UpdateChamCongRequest,
} from '../types/chamCong.types';

const BASE_URL = '/cham-cong';

type ApiResponse<T> = { data: T; succeeded: boolean; message: string };

export const chamCongApi = {
  getList: (
    thang: number,
    nam: number,
    cccd?: string,
    idPhongBan?: string
  ): Promise<ApiResponse<ChamCongDto[]>> =>
    axiosClient.get(BASE_URL, { params: { thang, nam, cccd, idPhongBan } }),

  getSummary: (
    thang: number,
    nam: number,
    idPhongBan?: string
  ): Promise<ApiResponse<ChamCongSummaryDto[]>> =>
    axiosClient.get(`${BASE_URL}/tong-hop`, { params: { thang, nam, idPhongBan } }),

  getCaLamViecTrongNgay: (
    cccd: string,
    ngay: string
  ): Promise<ApiResponse<{ gioVao: string | null; gioRa: string | null; isDayOff: boolean; source: string }>> =>
    axiosClient.get(`${BASE_URL}/ca-lam-viec-trong-ngay`, { params: { cccd, ngay } }),

  create: (
    data: CreateChamCongRequest
  ): Promise<ApiResponse<string>> =>
    axiosClient.post(BASE_URL, data),

  update: (
    id: string,
    data: UpdateChamCongRequest
  ): Promise<ApiResponse<boolean>> =>
    axiosClient.put(`${BASE_URL}/${id}`, data),

  delete: (id: string): Promise<ApiResponse<boolean>> =>
    axiosClient.delete(`${BASE_URL}/${id}`),

  import: (file: File): Promise<ApiResponse<ImportChamCongResultDto>> => {
    const formData = new FormData();
    formData.append('file', file);
    return axiosClient.post(`${BASE_URL}/import`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  downloadTemplate: (): void => {
    const csvContent = 'CCCD,NgayChamCong,GioVao,GioRa,GhiChu\n001234567890,15/07/2026,08:00,17:00,\n';
    const blob = new Blob(['\uFEFF' + csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'mau_cham_cong.csv';
    link.click();
    URL.revokeObjectURL(url);
  },
};
