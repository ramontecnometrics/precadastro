const getApiUrl = () => {
    // padrão
    let url = "https://h-api-cadastrodrhair.gestia.net.br";
 
    // ambiente de dev
    if (process.env.NODE_ENV === "development") {
       url = "https://localhost:5008";
    }
 
    if (!url) {
       throw new Error("Url da API não foi definida.");
    }
 
    return url;
 };
 
 export const ConnectionParams = {
    apiUrl: getApiUrl(),
 };
 