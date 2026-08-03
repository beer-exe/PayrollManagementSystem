import axiosClient from '@/services/api/axiosClient';
import {
  KhoanKhauTruDto,
  CreateKhoanKhauTruRequest,
  UpdateKhoanKhauTruRequest,
} from '../types';

const BASE = '/khoan-khau-tru';

export const khoanKhauTruApi = {
  getList: (isActive?: boolean): Promise<{ data: KhoanKhauTruDto[] }> => {
    const params: Record<string, unknown> = {};
    if (isActive !== undefined) params.isActive = isActive;
    return axiosClient.get(BASE, { params });
  },

  create: (payload: CreateKhoanKhauTruRequest): Promise<{ data: string; message: string }> =>
    axiosClient.post(BASE, payload),

  update: (id: string, payload: UpdateKhoanKhauTruRequest): Promise<{ data: boolean; message: string }> =>
    axiosClient.put(`${BASE}/${id}`, payload),

  delete: (id: string): Promise<{ data: boolean; message: string }> =>
    axiosClient.delete(`${BASE}/${id}`),
};
