import { api } from "./api/api.js";

export class PaymentModal {
  constructor() {
    this.modal = document.getElementById("payment-modal");
    this.form = document.getElementById("payment-form");
    this.closeBtn = document.getElementById("payment-close");
    this.cancelBtn = document.getElementById("payment-cancel");
    this.submitBtn = document.getElementById("payment-submit");
    this.methodSelect = document.getElementById("payment-method");
    this.cardFields = document.getElementById("card-fields");
    this.statusDiv = document.getElementById("payment-status");
    this.statusText = document.getElementById("payment-status-text");
    
    this.reservationData = null;
    this.onPaymentSuccess = null;
    this.onPaymentError = null;

    this.setupEventListeners();
  }

  setupEventListeners() {
    this.closeBtn.addEventListener("click", () => this.close());
    this.cancelBtn.addEventListener("click", () => this.close());
    this.form.addEventListener("submit", (e) => this.handleSubmit(e));
    this.methodSelect.addEventListener("change", () => this.updateMethodDisplay());
  }

  updateMethodDisplay() {
    const method = this.methodSelect.value;
    // Mostrar campos de tarjeta solo para métodos que los requieren
    if (method === "Card" || method === "Mock") {
      this.cardFields.style.display = "block";
    } else {
      this.cardFields.style.display = "none";
    }
  }

  open(reservationData) {
    this.reservationData = reservationData;
    this.statusDiv.classList.add("hidden");
    this.form.style.display = "block";
    this.submitBtn.disabled = false;

    // Poblate el formulario con datos de la reserva
    document.getElementById("payment-reservation-id").textContent = reservationData.id;
    document.getElementById("payment-amount").textContent = `$${reservationData.amount.toFixed(2)}`;
    
    // Resetear formulario
    this.form.reset();
    this.methodSelect.value = "";
    this.updateMethodDisplay();
    
    this.modal.classList.remove("hidden");
  }

  close() {
    this.modal.classList.add("hidden");
    this.statusDiv.classList.add("hidden");
  }

  showStatus(message, isError = false) {
    this.statusDiv.classList.remove("hidden");
    this.statusDiv.className = `${isError ? "bg-red-50 border border-red-200" : "bg-green-50 border border-green-200"}`;
    this.statusText.className = `text-sm ${isError ? "text-red-800" : "text-green-800"}`;
    this.statusText.textContent = message;
  }

  async handleSubmit(e) {
    e.preventDefault();
    
    const paymentMethod = this.methodSelect.value;
    if (!paymentMethod) {
      this.showStatus("Selecciona un método de pago", true);
      return;
    }

    this.submitBtn.disabled = true;
    this.showStatus("Procesando pago...", false);

    try {
      const cardData = {
        cardNumber: document.getElementById("card-number").value,
        cardholderName: document.getElementById("cardholder-name").value,
        expiryMonth: document.getElementById("expiry-month").value,
        expiryYear: document.getElementById("expiry-year").value,
        cvv: document.getElementById("cvv").value
      };

      const paymentResult = await api.processPayment(
        this.reservationData.id,
        this.reservationData.amount,
        paymentMethod,
        cardData
      );

      if (paymentResult.status === "Completed") {
        this.showStatus("✓ Pago completado exitosamente", false);
        this.form.style.display = "none";
        
        if (this.onPaymentSuccess) {
          setTimeout(() => {
            this.onPaymentSuccess(paymentResult);
            this.close();
          }, 2000);
        }
      } else if (paymentResult.status === "Failed") {
        this.showStatus(`✕ Pago rechazado: ${paymentResult.failureReason || "Error desconocido"}`, true);
        this.submitBtn.disabled = false;
        if (this.onPaymentError) {
          this.onPaymentError(paymentResult);
        }
      }
    } catch (error) {
      this.showStatus(`Error: ${error.message}`, true);
      this.submitBtn.disabled = false;
      if (this.onPaymentError) {
        this.onPaymentError(error);
      }
    }
  }
}
