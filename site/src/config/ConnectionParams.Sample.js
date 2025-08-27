const getApiUrl = () => {
    let url = null;
    if (process.env.NODE_ENV === 'development') {
        url = 'http://192.168.10.15:5005';
    }
    if (process.env.NODE_ENV === 'production') {
        if (process.env.REACT_APP_WEBSITE_NAME === 'tecnometrics') {
            url = 'http://ec2-3-209-57-166.compute-1.amazonaws.com:5001';
        }
    }
    if (!url) {
        throw new Error('Url da api não foi definida.');
    }
    return url;
};

export const ConnectionParams = {
    apiUrl: getApiUrl(),
};
