import axiosClient from '@/services/api/axiosClient';
import type { DonNghiDto, NgayPhepDto } from '../types/donNghi.types';

const BASE = '/don-nghi/me';

export interface CreateMyDonNghiRequest {
  loaiNghi: string;
  ngayBatDau: string;  // 'yyyy-MM-dd'
  ngayKetThuc: string; // 'yyyy-MM-dd'
  soNgayNghi: number;
  lyDo: string;
  taiLieuDinhKem?: string;
}

export const myDonNghiApi = {
  getMyList: (params: {
    thang?: number;
    nam?: number;
    trangThai?: string;
    loaiNghi?: string;
  }) =>
    axiosClient.get<unknown, { data: DonNghiDto[]; succeeded: boolean }>(BASE, { params }),

  createMy: (data: CreateMyDonNghiRequest) =>
    axiosClient.post<unknown, { data: string; succeeded: boolean }>(BASE, data),

  deleteMy: (id: string) =>
    axiosClient.delete<unknown, { data: boolean; succeeded: boolean }>(`${BASE}/${id}`),

  getMyNgayPhep: (nam: number) =>
    axiosClient.get<unknown, { data: NgayPhepDto | null; succeeded: boolean }>(`${BASE}/ngay-phep`, {
      params: { nam },
    }),
};
