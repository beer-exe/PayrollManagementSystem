import axiosClient from '../../../services/api/axiosClient';
import { ApiResponse } from '../../../types/api';

export interface KyChamCongDto {
  id: string;
  thang: number;
  nam: number;
  trangThai: string;
  trangThaiText: string;
}

export const kyChamCongApi = {
  getKyChamCong: (nam: number, thang: number): Promise<ApiResponse<KyChamCongDto>> => {
    return axiosClient.get(`/KyChamCong/${nam}/${thang}`);
  },

  chotCong: (nam: number, thang: number): Promise<ApiResponse<boolean>> => {
    return axiosClient.post(`/KyChamCong/chot-cong`, { nam, thang });
  },

  moChotCong: (nam: number, thang: number): Promise<ApiResponse<boolean>> => {
    return axiosClient.post(`/KyChamCong/mo-chot-cong`, { nam, thang });
  },
};
