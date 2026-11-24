# pip install cryptography

import base64
import json
from pathlib import Path
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.primitives.asymmetric.ed25519 import Ed25519PrivateKey


def generate_keypair(name: str, out_dir: Path) -> dict:
    private_key = Ed25519PrivateKey.generate()
    public_key = private_key.public_key()

    # Raw 32-byte seed (private) and 32-byte public key — matches NSec RawPrivateKey / RawPublicKey
    raw_private = private_key.private_bytes(
        encoding=serialization.Encoding.Raw,
        format=serialization.PrivateFormat.Raw,
        encryption_algorithm=serialization.NoEncryption(),
    )
    raw_public = public_key.public_bytes(
        encoding=serialization.Encoding.Raw,
        format=serialization.PublicFormat.Raw,
    )

    priv_path = out_dir / f"{name}_ed25519.key"
    pub_path = out_dir / f"{name}_ed25519.pub"

    priv_path.write_bytes(raw_private)
    pub_path.write_bytes(raw_public)

    return {
        "private_key_file": str(priv_path),
        "public_key_file": str(pub_path),
        "private_key_base64": base64.b64encode(raw_private).decode("ascii"),
        "public_key_base64": base64.b64encode(raw_public).decode("ascii"),
    }


def main() -> None:
    out_dir = Path("./keys")
    out_dir.mkdir(parents=True, exist_ok=True)

    server_keys = generate_keypair("server", out_dir)

    print(f"Keys written to: {out_dir.resolve()}\n")


if __name__ == "__main__":
    main()