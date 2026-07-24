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
  
  luongThoiGian: number;
  luongHieuSuatP3: number;
  
  phuCap: number;
  thuong: number;
  tangCa: number;
  
  phat: number;
  truBaoHiem: number;
  truThue: number;
  
  tongThuNhap: number;
  thucLinh: number;
  
  ghiChu?: string;
}

export interface CalculatePayrollCommand {
  thang: number;
  nam: number;
}
