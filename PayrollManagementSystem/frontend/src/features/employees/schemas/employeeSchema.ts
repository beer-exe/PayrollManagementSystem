import { z } from 'zod';

export const createEmployeeSchema = z.object({
  cccd: z.string().min(9, 'CCCD phải từ 9-12 ký tự').max(12, 'CCCD phải từ 9-12 ký tự'),
  hoTen: z.string().min(1, 'Họ tên không được để trống'),
  email: z.string().email('Email không hợp lệ').optional().or(z.literal('')),
  sdt: z.string().optional(),
  
  soHopDong: z.string().min(1, 'Số hợp đồng bắt buộc'),
  loaiHopDong: z.string().min(1, 'Vui lòng chọn loại hợp đồng'),
  
  ngayBatDauHopDong: z.string().min(1, 'Chọn ngày bắt đầu'),

  idPb: z.string().min(1, 'Vui lòng chọn phòng ban'),
  soQuyetDinh: z.string().min(1, 'Số quyết định bắt buộc'),
  idChucVu: z.string().min(1, 'Vui lòng chọn chức vụ'),
  idBacLuong: z.string().optional(),
  
  soBhxh: z.string().optional(),
  soBhyt: z.string().optional(),
  soTaiKhoan: z.string().optional(),
  tenNganHang: z.string().optional(),
  maSoThue: z.string().optional(),
});

export type CreateEmployeeFormValues = z.infer<typeof createEmployeeSchema>;

export const thanNhanSchema = z.object({
  maDinhDanh: z.string().optional().nullable(),
  tenTn: z.string().min(1, 'Tên người thân không được để trống'),
  ngaySinh: z.string().optional().nullable(),
  idMqh: z.string().uuid('Mối quan hệ không hợp lệ').optional().nullable().or(z.literal('')),
});

export type ThanNhanFormValues = z.infer<typeof thanNhanSchema>;

export const updateEmployeeSchema = z.object({
  cccd: z.string().min(1, 'Mã định danh không được để trống'),
  hoTen: z.string().min(1, 'Họ tên không được để trống'),
  email: z.string().email('Email không hợp lệ').optional().or(z.literal('')),
  sdt: z.string().optional().nullable(),
  gioiTinh: z.boolean().optional().nullable(),
  ngaySinh: z.string().optional().nullable(),
  danToc: z.string().optional().nullable(),
  diaChi: z.string().optional().nullable(),
  chuyenNganh: z.string().optional().nullable(),
  soBhxh: z.string().optional().nullable(),
  soBhyt: z.string().optional().nullable(),
  soTaiKhoan: z.string().optional().nullable(),
  tenNganHang: z.string().optional().nullable(),
  maSoThue: z.string().optional().nullable(),
  idPb: z.string().optional().nullable(),
  thanNhans: z.array(thanNhanSchema).optional().nullable(),
});

export type UpdateEmployeeFormValues = z.infer<typeof updateEmployeeSchema>;