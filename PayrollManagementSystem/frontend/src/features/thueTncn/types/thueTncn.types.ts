export interface BacThueDto {
  idBacThue: string;
  bac: number;
  tuGia: number;
  denGia: number | null;
  thueSuat: number;
  isActive: boolean;
}

export interface CauHinhGiamTruDto {
  idCauHinhGiamTru?: string;
  giamTruBanThan: number;
  giamTruNguoiPhuThuoc: number;
  ghiChu?: string;
}

export interface CreateBacThueRequest {
  bac: number;
  tuGia: number;
  denGia: number | null;
  thueSuat: number;
}

export interface UpdateBacThueRequest {
  tuGia: number;
  denGia: number | null;
  thueSuat: number;
  isActive: boolean;
}
