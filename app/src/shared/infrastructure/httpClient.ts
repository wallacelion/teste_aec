type ApiErrorPayload = {
    title?: string;
    message?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

const API_BASE_URL = (import.meta.env.VITE_API_URL || "http://localhost:5000").replace(/\/+$/, "");

export class ApiError extends Error {
    status: number;
    payload?: ApiErrorPayload;

    constructor(message: string, status: number, payload?: ApiErrorPayload) {
        super(message);
        this.name = "ApiError";
        this.status = status;
        this.payload = payload;
    }
}

const getErrorMessage = (payload?: ApiErrorPayload) => {
    if (!payload) {
        return "Falha ao processar a requisição.";
    }

    if (payload.message) {
        return payload.message;
    }

    if (payload.detail) {
        return payload.detail;
    }

    if (payload.title) {
        return payload.title;
    }

    return "Usuário ou senha inválidos.";
};

export const postJson = async <TResponse, TBody>(
    path: string,
    body: TBody,
    init?: RequestInit,
): Promise<TResponse> => {

    const response = await fetch(`${API_BASE_URL}${path}`, {
        method: "POST",
        ...init,
        headers: {
            "Content-Type": "application/json",
            ...(init?.headers || {}),
        },
        body: JSON.stringify(body),
    });

    if (!response.ok) {
        let payload: ApiErrorPayload | undefined;

        try {
            payload = (await response.json()) as ApiErrorPayload;
        } catch {
            payload = undefined;
        }

        throw new ApiError(getErrorMessage(payload), response.status, payload);
    }

    return (await response.json()) as TResponse;
};

export const getJson = async <TResponse>(
    path: string,
    init?: RequestInit,
): Promise<TResponse> => {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        method: "GET",
        ...init,
        headers: {
            "Content-Type": "application/json",
            ...(init?.headers || {}),
        },
    });

    if (!response.ok) {
        let payload: ApiErrorPayload | undefined;

        try {
            payload = (await response.json()) as ApiErrorPayload;
        } catch {
            payload = undefined;
        }

        throw new ApiError(getErrorMessage(payload), response.status, payload);
    }

    return (await response.json()) as TResponse;
};

export const putJson = async <TResponse, TBody>(
    path: string,
    body: TBody,
    init?: RequestInit,
): Promise<TResponse> => {

    const response = await fetch(`${API_BASE_URL}${path}`, {
        method: "PUT",
        ...init,
        headers: {
            "Content-Type": "application/json",
            ...(init?.headers || {}),
        },
        body: JSON.stringify(body),
    });

    if (!response.ok) {
        let payload: ApiErrorPayload | undefined;

        try {
            payload = (await response.json()) as ApiErrorPayload;
        } catch {
            payload = undefined;
        }

        throw new ApiError(getErrorMessage(payload), response.status, payload);
    }

    return (await response.json()) as TResponse;
};

export const pathJson = async <TResponse, TBody>(
    path: string,
    body: TBody,
    init?: RequestInit,
): Promise<TResponse> => {

    const response = await fetch(`${API_BASE_URL}${path}`, {
        method: "PATCH",
        ...init,
        headers: {
            "Content-Type": "application/json",
            ...(init?.headers || {}),
        },
        body: JSON.stringify(body),
    });

    if (!response.ok) {
        let payload: ApiErrorPayload | undefined;

        try {
            payload = (await response.json()) as ApiErrorPayload;
        } catch {
            payload = undefined;
        }

        throw new ApiError(getErrorMessage(payload), response.status, payload);
    }

    return (await response.json()) as TResponse;
};

export const deleteJson = async <TResponse>(
    path: string,
    init?: RequestInit,
): Promise<TResponse> => {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        method: "DELETE",
        ...init,
        headers: {
            "Content-Type": "application/json",
            ...(init?.headers || {}),
        },
    });

    if (!response.status === 204) {
        let payload: ApiErrorPayload | undefined;

        try {
            payload = (await response.json()) as ApiErrorPayload;
        } catch {
            payload = undefined;
        }

        throw new ApiError(getErrorMessage(payload), response.status, payload);
    }

    return (await response.json()) as TResponse;
};