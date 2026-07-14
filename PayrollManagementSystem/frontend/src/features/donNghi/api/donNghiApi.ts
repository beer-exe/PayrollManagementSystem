import axiosClient from '@/services/api/axiosClient';
import type {
  DonNghiDto,
  NgayPhepDto,
  CreateDonNghiRequest,
  TuChoiRequest,
  UpdateNgayPhepRequest,
} from '../types/donNghi.types';

const BASE_URL = '/don-nghi';
const PHEP_URL = '/ngay-phep';

export const donNghiApi = {
  getList: (params: {
    thang?: number;
    nam?: number;
    cccd?: string;
    trangThai?: string;
    idPhongBan?: string;
  }) =>
    axiosClient.get<{ data: DonNghiDto[]; succeeded: boolean }>(BASE_URL, { params }),

  create: (data: CreateDonNghiRequest) =>
    axiosClient.post<{ data: string; succeeded: boolean }>(BASE_URL, data),

  duyet: (id: string) =>
    axiosClient.patch<{ data: boolean; succeeded: boolean }>(`${BASE_URL}/${id}/duyet`),

  tuChoi: (id: string, body: TuChoiRequest) =>
    axiosClient.patch<{ data: boolean; succeeded: boolean }>(`${BASE_URL}/${id}/tu-choi`, body),

  delete: (id: string) =>
    axiosClient.delete<{ data: boolean; succeeded: boolean }>(`${BASE_URL}/${id}`),

  // Quota phép
  getNgayPhep: (nam: number, idPhongBan?: string) =>
    axiosClient.get<{ data: NgayPhepDto[]; succeeded: boolean }>(PHEP_URL, {
      params: { nam, idPhongBan },
    }),

  updateNgayPhep: (data: UpdateNgayPhepRequest) =>
    axiosClient.post<{ data: boolean; succeeded: boolean }>(PHEP_URL, data),
};
