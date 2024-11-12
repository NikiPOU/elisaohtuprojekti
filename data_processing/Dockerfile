FROM python:3-alpine3.15
WORKDIR /app
COPY . /app
EXPOSE 3000
RUN pip install -r requirements.txt
CMD flask run --port 3000