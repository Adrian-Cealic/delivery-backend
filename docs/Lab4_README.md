# Laborator 4 — Paternuri Structurale: Adapter, Composite, Façade

## Obiectiv

Implementarea a trei paternuri structurale în cadrul Delivery Management System:
- **Adapter** — compatibilitatea între clase cu interfețe diferite (gateway-uri de plată)
- **Composite** — model ierarhic pentru obiecte individuale și colecții tratate uniform (catalog produse)
- **Façade** — interfață simplificată către subsistem complex (plasare comandă)

---

## 1. Adapter Pattern

Sistemul integrează multiple gateway-uri de plată (PayPal, Stripe, Google Pay) cu API-uri externe diferite. Adapter-ul standardizează interfețele prin `IPaymentGateway`.

### API Endpoint
- `POST /api/payments/process` — procesează plată cu gateway selectat

---

## 2. Composite Pattern

Catalogul modelează produse individuale și categorii/submeniuri (bundles). Composite permite tratarea uniformă a structurii ierarhice prin `IProductCatalogComponent`, `ProductCatalogItem` (leaf), `ProductBundle` (composite).

### API Endpoint
- `GET /api/catalog` — returnează structura ierarhică

---

## 3. Façade Pattern

Plasarea unei comenzi implică: validare client, procesare plată, creare comandă, notificare. Façade oferă o singură metodă `PlaceOrder` care orchestrează fluxul.

### API Endpoint
- `POST /api/orders/place` — plasare comandă completă
