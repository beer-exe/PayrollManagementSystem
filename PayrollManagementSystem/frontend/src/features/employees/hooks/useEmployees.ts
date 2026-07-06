import { useState, useCallback } from 'react';
import { message } from 'antd';
import { employeeApi } from '../api/employeeApi';
import { ChangeStatusDto, CreateEmployeeCommand, UpdateEmployeeCommand } from '../types/employee.types';
import { UserProfileDetail } from '@/types/profile.types';

export const useEmployees = () => {
  const [employees, setEmployees] = useState<UserProfileDetail[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalRecords, setTotalRecords] = useState(0);

  const fetchEmployees = useCallback(async (searchTerm: string, pageNumber: number, pageSize: number) => {
    setLoading(true);
    try {
      const response = await employeeApi.getEmployees({ SearchTerm: searchTerm, PageNumber: pageNumber, PageSize: pageSize });
      if (response.succeeded) {
        setEmployees(response.data);
        setTotalRecords(response.totalRecords);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi tải danh sách nhân viên');
    } finally {
      setLoading(false);
    }
  }, []);

  const [isExporting, setIsExporting] = useState(false);

  const exportExcel = async (searchTerm?: string) => {
    setIsExporting(true);
    try {
      const response = await employeeApi.exportExcel({ searchTerm });
      const url = window.URL.createObjectURL(new Blob([response as unknown as BlobPart]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `NhanVien_${new Date().getTime()}.xlsx`);
      document.body.appendChild(link);
      link.click();
      message.success('Xuất file Excel thành công');
    } catch (error) {
      message.error('Lỗi khi xuất file Excel');
    } finally {
      setIsExporting(false);
    }
  };

  const createEmployee = async (data: CreateEmployeeCommand): Promise<boolean> => {
    try {
      const res = await employeeApi.createEmployee(data);
      if (res.succeeded) {
        message.success('Thêm mới nhân viên thành công!');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi thêm mới nhân viên');
      return false;
    }
  };

  const updateEmployee = async (cccd: string, data: UpdateEmployeeCommand): Promise<boolean> => {
    try {
      const res = await employeeApi.updateEmployee(cccd, data);
      if (res.succeeded) {
        message.success('Cập nhật hồ sơ nhân viên thành công!');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi cập nhật hồ sơ');
      return false;
    }
  };

  const changeStatus = async (cccd: string, payload: ChangeStatusDto): Promise<boolean> => {
    try {
      const res = await employeeApi.changeStatus(cccd, payload);
      if (res.succeeded) {
        message.success('Cập nhật trạng thái thành công!');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi cập nhật trạng thái');
      return false;
    }
  };

return { employees, totalRecords, loading, isExporting, fetchEmployees, exportExcel, createEmployee, changeStatus, updateEmployee };
};