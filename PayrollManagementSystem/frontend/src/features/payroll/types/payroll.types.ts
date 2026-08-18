export interface PayrollListDto {
  idBangLuong: string;
  idKyLuong: string;
  cccdNhanVien: string;
  tenNhanVien: string;
  tenPhongBan: string;
  tenChucVu: string;
  
  thang: number;
  nam: number;
  
  p1: number;
  heSoP2: number;
  heSoP3: number;
  
  ngayCongChuan: number;
  ngayCongThucTe: number;
  
  gioCongChuan: number;
  gioCongThucTe: number;
  
  luongThoiGian: number;
  luongHieuSuatP3: number;
  
  phuCap: number;
  thuong: number;
  tangCa: number;
  
  phat: number;
  khauTru: number;
  truThue: number;
  
  tongThuNhap: number;
  thucLinh: number;
  
  ghiChu?: string;
  chiTietKhauTru?: string;
  chiTietThue?: string;

  trangThai: string;
  trangThaiText: string;
  lyDoKhieuNai?: string;
  phanHoiKhieuNai?: string;
}

export interface CalculatePayrollCommand {
  thang: number;
  nam: number;
}

export interface ReopenPayrollCommand {
  thang: number;
  nam: number;
  lyDo?: string;
}

export interface KyLuongStatusDto {
  thang: number;
  nam: number;
  tenKyLuong: string;
  ngayBatDau: string;
  ngayKetThuc: string;
  trangThai: 'CHUA_TAO' | 'CHUA_CHOT' | 'DA_CHOT' | 'DA_THANH_TOAN' | string;
  tenTrangThai: string;
  isLocked: boolean;
  coDuLieuBangLuong: boolean;
}
