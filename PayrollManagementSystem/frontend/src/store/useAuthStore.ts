import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { UserProfile } from '@/types/auth.types';

interface AuthState {
  user: UserProfile | null;
  isAuthenticated: boolean;
  isSessionExpired: boolean;
  login: (user: UserProfile) => void;
  logout: () => void;
  setSessionExpired: (status: boolean) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,
      isSessionExpired: false,
      
      login: (user) => set({ 
        user, 
        isAuthenticated: true, 
        isSessionExpired: false 
      }),
      
      logout: () => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        set({ user: null, isAuthenticated: false, isSessionExpired: false });
      },
      
      setSessionExpired: (status) => set({ 
        isSessionExpired: status 
      }),
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({ 
        user: state.user, 
        isAuthenticated: state.isAuthenticated 
      }),
    }
  )
);