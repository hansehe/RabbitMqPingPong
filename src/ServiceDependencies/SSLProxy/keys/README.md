# Generate Self-Signed Certificates

- Original source: [https://www.rabbitmq.com/ssl.html#manual-certificate-generation](https://www.rabbitmq.com/ssl.html#manual-certificate-generation)

All steps are performed in a linux (ubuntu) terminal.

1. Install openssl:
    - `sudo apt-get install openssl`
2. Generate keys:
    - Remove windows carriage return: 
        - `sed -i 's/\r//g' generateKeys.sh`
    - `chmod +x ./generateKeys.sh`
    - `./generateKeys.sh`

Hostname for server/client certificates are by default set to:
- rabbit

Password for certificates are by default set to:
- password

Minor explanations:
- `ca_certificate_bundle.pem` and `ca_certificate_bundle.cer` are the root CA certificates with the same information, but in PEM and DER format. The PEM is the most commonly used. Use `ca_certificate_bundle.pem` as the CA certificate with RabbitMq.
- The `client` and `server` folder contains a private key (`private_key.pem`) and a certificate (`client_certificate.pem`/`server_certificate.pem`). Use the server certificate and the private key with rabbitMq. The client certificate/private key are used by the client for peer verification.
- The .p12 file is a combination of the private key and certificate, and commonly used by dotnet applications. Use the `client_certificate.p12` with your dotnet application code.

