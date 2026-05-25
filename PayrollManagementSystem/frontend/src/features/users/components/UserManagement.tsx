import React, { useState } from 'react';
import { Switch, Modal, message } from 'antd';
import { useUsers } from '../hooks/useUsers';
import { CreateUserModal } from './CreateUserModal';
import { UpdateRoleModal } from './UpdateRoleModal';
import { UserDto } from '../types/user.types';

export const UserManagement: React.FC = () => {
  const { users, roles, isLoading, handleCreateUser, handleUpdateRole, handleToggleStatus, handleResetPassword } = useUsers();
  
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [roleModalUser, setRoleModalUser] = useState<UserDto | null>(null);

  const onResetPasswordClick = (user: UserDto) => {
    Modal.confirm({
      title: 'Xác nhận đặt lại mật khẩu',
      content: `Hệ thống sẽ đặt lại mật khẩu cho tài khoản ${user.tenTaiKhoan} và gửi email thông báo.`,
      okText: 'Xác nhận',
      cancelText: 'Hủy',
      okButtonProps: { className: 'bg-red-600 hover:bg-red-700 border-none' },
      onOk: async () => {
        // Trong thực tế, mật khẩu nên được tạo ngẫu nhiên từ backend hoặc có form nhập riêng.
        await handleResetPassword(user.idTaiKhoan, { idTaiKhoan: user.idTaiKhoan, newPassword: 'NewPassword@123' });
      }
    });
  };

  return (
    <div className="flex flex-col h-full w-full relative overflow-hidden bg-transparent p-4 sm:p-6">
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 dark:border-gray-700 flex flex-col flex-1 overflow-hidden transition-colors">
        
        {/* Toolbar */}
        <div className="flex items-center justify-between p-4 border-b border-gray-100 dark:border-gray-700">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Quản lý Tài khoản</h2>
          <button 
            onClick={() => setIsCreateOpen(true)}
            className="inline-flex items-center justify-center rounded-md text-sm font-medium bg-violet-600 text-white hover:bg-violet-700 px-4 py-2 transition-colors"
          >
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5 mr-2">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
            </svg>
            Thêm tài khoản
          </button>
        </div>

        {/* Data Table */}
        <div className="flex-1 overflow-auto relative">
          {isLoading && (
            <div className="absolute inset-0 bg-white/60 dark:bg-gray-800/60 backdrop-blur-[1px] z-20 flex items-center justify-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-violet-600"></div>
            </div>
          )}
          <table className="w-full text-left border-collapse min-w-max">
            <thead>
              <tr>
                <th className="sticky top-0 bg-gray-50 dark:bg-gray-700/90 backdrop-blur-sm px-4 py-3 text-sm font-semibold text-gray-700 dark:text-gray-200 border-b border-gray-200 dark:border-gray-600 z-10">Tên đăng nhập</th>
                <th className="sticky top-0 bg-gray-50 dark:bg-gray-700/90 backdrop-blur-sm px-4 py-3 text-sm font-semibold text-gray-700 dark:text-gray-200 border-b border-gray-200 dark:border-gray-600 z-10">Email / Họ tên</th>
                <th className="sticky top-0 bg-gray-50 dark:bg-gray-700/90 backdrop-blur-sm px-4 py-3 text-sm font-semibold text-gray-700 dark:text-gray-200 border-b border-gray-200 dark:border-gray-600 z-10">Vai trò</th>
                <th className="sticky top-0 bg-gray-50 dark:bg-gray-700/90 backdrop-blur-sm px-4 py-3 text-sm font-semibold text-gray-700 dark:text-gray-200 border-b border-gray-200 dark:border-gray-600 z-10 text-center">Trạng thái</th>
                <th className="sticky top-0 bg-gray-50 dark:bg-gray-700/90 backdrop-blur-sm px-4 py-3 text-sm font-semibold text-gray-700 dark:text-gray-200 border-b border-gray-200 dark:border-gray-600 z-10 text-right">Hành động</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.idTaiKhoan} className="hover:bg-gray-50 dark:hover:bg-gray-700/30 transition-colors">
                  <td className="px-4 py-3 text-sm font-medium text-gray-900 dark:text-white border-b border-gray-100 dark:border-gray-700/50">
                    {user.tenTaiKhoan}
                  </td>
                  <td className="px-4 py-3 text-sm text-gray-600 dark:text-gray-300 border-b border-gray-100 dark:border-gray-700/50">
                    <div className="font-medium text-gray-800 dark:text-gray-200">{user.hoTen}</div>
                    <div className="text-xs text-gray-400">{user.email}</div>
                  </td>
                  <td className="px-4 py-3 text-sm text-gray-600 dark:text-gray-300 border-b border-gray-100 dark:border-gray-700/50">
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
                      {user.tenVaiTro}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-sm text-center border-b border-gray-100 dark:border-gray-700/50">
                    <Switch 
                      checked={user.trangThai === 'HOAT_DONG'} 
                      onChange={() => handleToggleStatus(user.idTaiKhoan)}
                      checkedChildren="Mở"
                      unCheckedChildren="Khóa"
                      className={user.trangThai === 'HOAT_DONG' ? 'bg-green-500' : 'bg-gray-400'}
                    />
                  </td>
                  <td className="px-4 py-3 text-sm text-right border-b border-gray-100 dark:border-gray-700/50 space-x-3">
                    <button onClick={() => setRoleModalUser(user)} className="text-violet-600 hover:text-violet-900 dark:text-violet-400 dark:hover:text-violet-300 font-medium">
                      Đổi quyền
                    </button>
                    <button onClick={() => onResetPasswordClick(user)} className="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300 font-medium">
                      Đặt lại MK
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Render Modals */}
      <CreateUserModal 
        isOpen={isCreateOpen} 
        onClose={() => setIsCreateOpen(false)} 
        onSubmit={handleCreateUser} 
        roles={roles}
      />
      
      <UpdateRoleModal 
        user={roleModalUser}
        isOpen={!!roleModalUser}
        onClose={() => setRoleModalUser(null)}
        onSubmit={handleUpdateRole}
        roles={roles}
      />
    </div>
  );
};