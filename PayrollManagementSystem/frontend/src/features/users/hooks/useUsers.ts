import { useState, useEffect, useCallback } from 'react';
import { message } from 'antd';
import { userApi } from '../api/userApi';
import { UserDto, RoleDto, CreateUserCommand, UpdateUserRoleCommand, ResetPasswordCommand, EmployeeNoAccount } from '../types/user.types';

export const useUsers = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);

const fetchData = useCallback(async () => {
    setIsLoading(true);
    try {
      const [usersRes, rolesRes] = await Promise.all([
        userApi.getUsers(),
        userApi.getRoles()
      ]);
      
      if (usersRes.succeeded) setUsers(usersRes.data);
      if (rolesRes.succeeded) setRoles(rolesRes.data);
      
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi tải dữ liệu.');
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const handleCreateUser = async (data: CreateUserCommand): Promise<boolean> => {
    try {
      const res = await userApi.createUser(data);
      if (res.succeeded) {
        message.success('Tạo tài khoản thành công!');
        fetchData();
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi tạo tài khoản.');
      return false;
    }
  };

  const handleUpdateRole = async (id: string, data: UpdateUserRoleCommand): Promise<boolean> => {
    try {
      const res = await userApi.updateRole(id, data);
      if (res.succeeded) {
        message.success('Cập nhật quyền thành công!');
        fetchData();
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi cập nhật quyền.');
      return false;
    }
  };

  const handleToggleStatus = async (id: string) => {
    try {
      const res = await userApi.toggleStatus(id);
      if (res.succeeded) {
        message.success('Đã thay đổi trạng thái tài khoản!');
        fetchData();
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi thay đổi trạng thái.');
    }
  };

  const handleResetPassword = async (id: string, data: ResetPasswordCommand): Promise<boolean> => {
    try {
      const res = await userApi.resetPassword(id, data);
      if (res.succeeded) {
        message.success('Đặt lại mật khẩu thành công! Đã gửi thông báo qua email.');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error?.response?.data?.Message || 'Lỗi khi đặt lại mật khẩu.');
      return false;
    }
  };

  return {
    users,
    roles,
    isLoading,
    handleCreateUser,
    handleUpdateRole,
    handleToggleStatus,
    handleResetPassword,
    refreshUsers: fetchData
  };
};

export const useEmployeesNoAccount = (isOpen: boolean) => {
  const [employees, setEmployees] = useState<EmployeeNoAccount[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!isOpen) return;

    const fetchEmployees = async () => {
      setIsLoading(true);
      try {
        const response = await userApi.getEmployeesNoAccount();
        
        if (response && response.data) {
           setEmployees(response.data);
        } else {
           setEmployees([]);
        }
      } catch (error) {
        console.error("Lỗi:", error);
        setEmployees([]);
      } finally {
        setIsLoading(false);
      }
    };

    fetchEmployees();
  }, [isOpen]);

  return { employees, isLoading };
};