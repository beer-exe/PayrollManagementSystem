export interface KyKpi {
  idKyKpi: string;
  tenKyKpi: string;
  thang: number;
  nam: number;
  trangThai: string;
  trangThaiValue: number;
  tongSoPhieu: number;
  soPhieuDaDuyet: number;
}

export interface PhieuKpi {
  idPhieuKpi: string;
  idKyKpi: string;
  tenKyKpi: string;
  thang: number;
  nam: number;
  tongDiemKpi: number;
  heSoP3: number;
  trangThai: string;
  trangThaiValue: number;
  nhanXet: string | null;
  cccdNhanVien: string;
  tenNhanVien: string;
  canManage: boolean;
}

export interface ChiTietKpi {
  idChiTietKpi?: string;
  idPhieuKpi?: string;
  mucTieu: string;
  donViTinh: string;
  trongSo: number;
  chiTieu: number;
  thucTe: number;
  tiLeHoanThanh?: number;
  diemKpi?: number;
  loaiTieuChi?: string | number;
  loaiTieuChiValue?: string | number;
}

export interface PhieuKpiDetail {
  idPhieuKpi: string;
  idKyKpi: string;
  tenKyKpi: string;
  thang: number;
  nam: number;
  cccdNhanVien: string;
  tenNhanVien: string;
  cccdQuanLy: string | null;
  tenQuanLy: string | null;
  tongDiemKpi: number;
  heSoP3: number;
  nhanXet: string | null;
  trangThai: string;
  trangThaiValue: number;
  canManage?: boolean;
  chiTietKpis: ChiTietKpi[];
}

export interface CreateKyKpiRequest {
  tenKyKpi: string;
  thang: number;
  nam: number;
}

export interface ApproveKpiRequest {
  nhanXet?: string;
}
