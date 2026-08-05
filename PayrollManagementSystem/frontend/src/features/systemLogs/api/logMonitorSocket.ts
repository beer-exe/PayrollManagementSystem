import { HubConnectionBuilder, HubConnection, LogLevel } from '@microsoft/signalr';
import type { SystemLogDto } from '../types';

const BASE_URL = import.meta.env.VITE_API_BASE_URL?.replace('/api', '') || '';

export function createLogMonitorConnection(token: string): HubConnection {
  return new HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/monitor`, {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();
}

export type { HubConnection };

export interface RealtimeLogEvent {
  raiseDate: string;
  level: string;
  message: string | null;
  exception: string | null;
}

export function mapRealtimeToDto(event: RealtimeLogEvent, id: number): SystemLogDto {
  return {
    id,
    raiseDate: event.raiseDate,
    level: event.level,
    message: event.message,
    exception: event.exception,
    properties: null,
  };
}
