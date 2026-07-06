export interface ChiTietDanhGiaDto {
  idChiTiet: string;
  idTieuChi: string;
  tenNangLuc: string;
  yeuCauToiThieu: string;
  tyTrong: number;
  diemTuDanhGia: number | null;
  diemQuanLyDanhGia: number | null;
  nhanXetNhanVien: string | null;
  nhanXetQuanLy: string | null;
}

export interface PhieuDanhGiaDto {
  idPhieu: string;
  idKyDanhGia: string;
  tenKyDanhGia: string;
  cccdNhanVien: string;
  diemTongHop: number | null;
  heSoP2: number | null;
  xepLoai: string | null;
  nhanXetChung: string | null;
  trangThai: string;
  canEvaluate: boolean;
  chiTietDanhGias: ChiTietDanhGiaDto[];
}

export interface GenerateMyPhieuDanhGiaCommand {
  idKyDanhGia: string;
}

export interface ChiTietTuDanhGiaDto {
  idChiTiet: string;
  diemTuDanhGia: number;
  nhanXetNhanVien: string | null;
}

export interface SubmitTuDanhGiaCommand {
  idPhieu: string;
  isSubmit: boolean;
  chiTiets: ChiTietTuDanhGiaDto[];
}

export interface ChiTietManagerEvaluationDto {
  idChiTiet: string;
  diemQuanLyDanhGia: number;
  nhanXetQuanLy: string | null;
}

export interface SubmitManagerEvaluationCommand {
  idPhieu: string;
  isSubmit: boolean;
  nhanXetChung: string | null;
  chiTiets: ChiTietManagerEvaluationDto[];
}
