hostname=rabbit
password=password

echo hostname: $hostname
echo password: $password

mkdir testca
cp openssl.cnf testca/openssl.cnf 

cd testca
mkdir certs private
chmod 700 private
echo 01 > serial
touch index.txt


openssl req -x509 -config openssl.cnf -newkey rsa:2048 -days 365000 \
    -out ca_certificate_bundle.pem -outform PEM -subj /CN=MyTestCA/ -nodes
openssl x509 -in ca_certificate_bundle.pem -out ca_certificate_bundle.cer -outform DER

cd ..
ls
# => testca
mkdir server
cd server
openssl genrsa -out private_key.pem 2048
openssl req -new -key private_key.pem -out req.pem -outform PEM \
    -subj /CN=$hostname/O=server/ -nodes
cd ../testca
openssl ca -config openssl.cnf -in ../server/req.pem -out \
    ../server/server_certificate.pem -notext -batch -extensions server_ca_extensions
cd ../server
openssl pkcs12 -export -out server_certificate.p12 -in server_certificate.pem -inkey private_key.pem \
    -passout pass:$password

cd ..
ls
# => server testca
mkdir client
cd client
openssl genrsa -out private_key.pem 2048
openssl req -new -key private_key.pem -out req.pem -outform PEM \
    -subj /CN=$hostname/O=client/ -nodes
cd ../testca
openssl ca -config openssl.cnf -in ../client/req.pem -out \
    ../client/client_certificate.pem -notext -batch -extensions client_ca_extensions
cd ../client
openssl pkcs12 -export -out client_certificate.p12 -in client_certificate.pem -inkey private_key.pem \
    -passout pass:$password
