export interface KyDanhGiaDto {
  idKyDanhGia: string;
  tenKyDanhGia: string;
  nam: number;
  ngayBatDau: string;
  ngayKetThuc: string;
  trangThai: string;
}

export interface CreateKyDanhGiaDto {
  tenKyDanhGia: string;
  ngayBatDau: string;
  ngayKetThuc: string;
}

export interface ChangeStatusKyDanhGiaCommand {
  idKyDanhGia: string;
  trangThaiMoi: string | number; // Actually Enum in backend: 0: DANG_MO, 1: DA_DONG, 2: DA_HUY. Let's use numbers.
}
