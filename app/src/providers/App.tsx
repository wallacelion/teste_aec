import { createContext, useState } from "react";
import type { User } from "../shared/types/User";
import type { Theme } from "../shared/types/Theme";

interface AppContentProps {
    user: User | null,
    setUser: React.Dispatch<React.SetStateAction<User | null>>,
    theme: Theme | null,
    setTheme: React.Dispatch<React.SetStateAction<Theme | null>>,
    tokenAuth: string | null,
    setTokenAuth: React.Dispatch<React.SetStateAction<string | null>>,
    isLoading: boolean,
    setIsLoading: React.Dispatch<React.SetStateAction<boolean>>,
    breadcrumbMenu: any[] | null,
    setBreadcrumbMenu: React.Dispatch<React.SetStateAction<any[] | null>>,
}

interface AppProvidersProps {
    children: any
}

export const AppContent = createContext<AppContentProps | undefined>(undefined);

export const AppProviders = ({ children }: AppProvidersProps) => {

    const [user, setUser] = useState<User | null>(null);
    const [theme, setTheme] = useState<Theme | null>(null);
    const [tokenAuth, setTokenAuth] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [breadcrumbMenu, setBreadcrumbMenu] = useState<any[]>([]);

    return <AppContent.Provider
        value={{
            user,
            setUser,
            theme,
            setTheme,
            tokenAuth,
            setTokenAuth,
            isLoading,
            setIsLoading,
            breadcrumbMenu,
            setBreadcrumbMenu,
        }}>
        {children}
    </AppContent.Provider>
}