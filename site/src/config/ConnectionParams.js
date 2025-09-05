const getApiUrl = () => {
   const url = import.meta.env.VITE_API_URL;

   if (!url) {
      throw new Error('Url da API não foi definida.');
   }

   return url;
};

export const ConnectionParams = {
   apiUrl: getApiUrl(),
};
