import axios from "axios";

const urlBase = "https://viacep.com.br/ws/";

 class ConsultaDeCep {
	constructor() {
        this.cache = [];

		this.insertToCache = this.insertToCache.bind(this);
		this.getFromCache = this.getFromCache.bind(this);
		this.get = this.get.bind(this);
	}

	insertToCache(url, data) {
		let indice = -1;
		this.cache.map((item, index) => {
			if (item.url === url) {
				indice = index;
			}
			return true;
		});

		if (indice === -1) {
			this.cache.push({
				url: url,
				data: data,
			});
		}
	}

	getFromCache(url) {
		let result = null;
		let indice = -1;
		this.cache.map((item, index) => {
			if (item.url === url) {
				indice = index;
			}
			return true;
		});

		if (indice >= 0) {
			result = this.cache[indice];
		}

		return result;
	}

	get(cep, useCache = false) {
		return new Promise((resolve, reject) => {
			let achouNoCache = false;
			let cachedResult = null;

            let url = urlBase + cep + '/json';

			if (useCache) {
				cachedResult = this.getFromCache(url);
				achouNoCache = cachedResult ? true : false;
			}

			if (!achouNoCache) {
				axios
					.get(url)
					.then((result) => {
						if (useCache) {
							this.insertToCache(url, result.data);
						}
						resolve( result.data);
					})
					.catch((e) => this.handleErrorMessage(e, this, reject));
			} else {
				resolve(cachedResult.data);
			}
		});
	}

	handleErrorMessage(e, sender, reject) {
		let mensagem = "";
		if (e.response && e.response.data && e.response.data.errorMessage) {
			mensagem = e.response.data.errorMessage;
		} else if (e.response && e.response.data && e.response.data.ExceptionMessage) {
			mensagem = e.response.request.response;
		} else if (e.response && e.response.request && e.response.request.response) {
			mensagem = e.response.request.response;
		} else if (e.response && e.response.statusText) {
			mensagem = e.response.statusText;
		} else {
			mensagem = e.message;
		}

		if (mensagem === "Network Error") {
			mensagem = "O serviço de consulta de CEP está indisponível.";
		}

		if (reject) {
			reject(mensagem);
		}
	}
}

const consultaDeCep = new ConsultaDeCep();
export default consultaDeCep;