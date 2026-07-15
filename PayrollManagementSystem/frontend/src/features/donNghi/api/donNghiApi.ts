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
    axiosClient.get<unknown, { data: DonNghiDto[]; succeeded: boolean }>(BASE_URL, { params }),

  create: (data: CreateDonNghiRequest) =>
    axiosClient.post<unknown, { data: string; succeeded: boolean }>(BASE_URL, data),

  duyet: (id: string) =>
    axiosClient.patch<unknown, { data: boolean; succeeded: boolean }>(`${BASE_URL}/${id}/duyet`),

  tuChoi: (id: string, body: TuChoiRequest) =>
    axiosClient.patch<unknown, { data: boolean; succeeded: boolean }>(`${BASE_URL}/${id}/tu-choi`, body),

  delete: (id: string) =>
    axiosClient.delete<unknown, { data: boolean; succeeded: boolean }>(`${BASE_URL}/${id}`),

  // Ngày phép
  getNgayPhep: (nam: number, idPhongBan?: string) =>
    axiosClient.get<unknown, { data: NgayPhepDto[]; succeeded: boolean }>(PHEP_URL, {
      params: { nam, idPhongBan },
    }),

  updateNgayPhep: (data: UpdateNgayPhepRequest) =>
    axiosClient.post<unknown, { data: boolean; succeeded: boolean }>(PHEP_URL, data),
};
