import { postJson } from "../../../shared/infrastructure/httpClient";

export interface LoginPayload {
    username: string;
    password: string;
}

interface LoginResponseRaw {
    statusCode?: string;
    status?: string;
    data?: any;
}

export interface LoginResponse {
    accessToken: string | null;
    refreshToken: string | null;
}

const normalizeLoginResponse = (response: LoginResponseRaw): LoginResponse => {
    const data: any = response?.data;
    return {
        accessToken: data.accessToken || data.AccessToken || null,
        refreshToken: data.refreshToken || data.RefreshToken || null,
    };
};

export const loginRequest = async (payload: LoginPayload): Promise<LoginResponse> => {
    const raw = await postJson<LoginResponseRaw, LoginPayload>("/api/auth/login", payload);
    return normalizeLoginResponse(raw);
};