import { useState, useEffect, useCallback } from 'react';
import { userApi } from '../api/userApi';
import { UserDto, RoleDto, CreateUserCommand, UpdateUserRoleCommand, ResetPasswordCommand, EmployeeNoAccount } from '../types/user.types';

export const useUsers = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const showToast = useCallback((msg: string, type: 'success' | 'error') => {
    setToast({ message: msg, type });
  }, []);

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
      showToast(error?.response?.data?.Message || 'Lỗi khi tải dữ liệu.', 'error');
    } finally {
      setIsLoading(false);
    }
  }, [showToast]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const handleCreateUser = async (data: CreateUserCommand): Promise<boolean> => {
    try {
      const res = await userApi.createUser(data);
      if (res.succeeded) {
        showToast('Tạo tài khoản thành công!', 'success');
        fetchData();
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi tạo tài khoản.', 'error');
      return false;
    }
  };

  const handleUpdateRole = async (id: string, data: UpdateUserRoleCommand): Promise<boolean> => {
    try {
      const res = await userApi.updateRole(id, data);
      if (res.succeeded) {
        showToast('Cập nhật quyền thành công!', 'success');
        fetchData();
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi cập nhật quyền.', 'error');
      return false;
    }
  };

  const handleToggleStatus = async (id: string) => {
    try {
      const res = await userApi.toggleStatus(id);
      if (res.succeeded) {
        showToast('Đã thay đổi trạng thái tài khoản!', 'success');
        fetchData();
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi thay đổi trạng thái.', 'error');
    }
  };

  const handleResetPassword = async (id: string, data: ResetPasswordCommand): Promise<boolean> => {
    try {
      const res = await userApi.resetPassword(id, data);
      if (res.succeeded) {
        showToast('Đặt lại mật khẩu thành công! Đã gửi thông báo qua email.', 'success');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi đặt lại mật khẩu.', 'error');
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
    refreshUsers: fetchData,
    toast,
    setToast
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