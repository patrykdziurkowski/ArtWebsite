FROM mcr.microsoft.com/mssql/server:2022-latest
EXPOSE 1433

# Add the directory containing sqlcmd to the PATH for easier debugging access
ENV PATH="/opt/mssql-tools18/bin:${PATH}"

ENV ACCEPT_EULA=Y
ARG password
RUN test -n "${password}" || (echo "password not set" && false)
ENV MSSQL_SA_PASSWORD=${password}

CMD ["/opt/mssql/bin/sqlservr"]

HEALTHCHECK --interval=1s --timeout=5s --retries=90 \
        CMD /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -Q "SELECT 1" -b -No