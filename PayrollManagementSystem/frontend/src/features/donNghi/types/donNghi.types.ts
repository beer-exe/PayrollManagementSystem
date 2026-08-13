export interface DonNghiDto {
  id: string;
  cccdNhanVien: string;
  hoTenNhanVien: string;
  tenPhongBan?: string;
  loaiNghi: string;
  ngayBatDau: string;
  ngayKetThuc: string;
  soNgayNghi: number;
  lyDo: string;
  taiLieuDinhKem?: string;
  trangThai: string;
  hoTenNguoiDuyet?: string;
  lyDoTuChoi?: string;
  ngayDuyet?: string;
  ngayTao?: string;
}

export interface NgayPhepDto {
  id: string;
  cccdNhanVien: string;
  hoTenNhanVien: string;
  tenPhongBan?: string;
  nam: number;
  tongNgayPhep: number;
  daSuDung: number;
  conLai: number;
}

export interface CreateDonNghiRequest {
  cccdNhanVien: string;
  loaiNghi: string;
  ngayBatDau: string;
  ngayKetThuc: string;
  soNgayNghi: number;
  lyDo: string;
  taiLieuDinhKem?: string;
}

export interface TuChoiRequest {
  lyDoTuChoi: string;
}

export interface UpdateNgayPhepRequest {
  cccdNhanVien: string;
  nam: number;
  tongNgayPhep: number;
}

export const LOAI_NGHI_OPTIONS = [
  { value: 'NGHI_PHEP_NAM', label: 'Nghỉ phép năm' },
  { value: 'NGHI_KHONG_LUONG', label: 'Nghỉ không lương' },
  { value: 'NGHI_OM_DAU', label: 'Nghỉ ốm đau' },
  { value: 'NGHI_THAI_SAN', label: 'Nghỉ thai sản' },
  { value: 'NGHI_CHE_DO', label: 'Nghỉ theo chế độ' },
];
