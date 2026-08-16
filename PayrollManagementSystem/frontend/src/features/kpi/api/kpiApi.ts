import axiosClient from '../../../services/api/axiosClient';
import {
  KyKpi,
  PhieuKpi,
  PhieuKpiDetail,
  CreateKyKpiRequest,
  ApproveKpiRequest,
  ChiTietKpi
} from '../types/kpi.types';

export interface AssignKpiRequest {
  chiTietKpis: ChiTietKpi[];
}

export const kpiApi = {
  getKyKpis: () => {
    return axiosClient.get<KyKpi[]>('/kpi/ky-kpi');
  },

  createKyKpi: (data: CreateKyKpiRequest) => {
    return axiosClient.post<string>('/kpi/ky-kpi', data);
  },

  getPhieuKpisByTaiKhoan: (taiKhoanId: string) => {
    return axiosClient.get<PhieuKpi[]>(`/kpi/nhan-vien/${taiKhoanId}`);
  },

  getPhieuKpisByKy: (idKyKpi: string) => {
    return axiosClient.get<PhieuKpi[]>(`/kpi/ky-kpi/${idKyKpi}/phieu`);
  },

  getPhieuKpiDetail: (id: string) => {
    return axiosClient.get<PhieuKpiDetail>(`/kpi/phieu/${id}`);
  },

  saveChiTietKpi: (idPhieuKpi: string, data: ChiTietKpi[]) => {
    return axiosClient.post(`/kpi/phieu/${idPhieuKpi}/chi-tiet`, data);
  },

  assignKpi: (idPhieuKpi: string, data: AssignKpiRequest) => {
    return axiosClient.post(`/kpi/phieu/${idPhieuKpi}/assign`, data);
  },

  submitPhieuKpi: (id: string) => {
    return axiosClient.post<string>(`/kpi/phieu/${id}/submit`);
  },

  approvePhieuKpi: (id: string, data: ApproveKpiRequest) => {
    return axiosClient.post<string>(`/kpi/phieu/${id}/approve`, data);
  }
};
